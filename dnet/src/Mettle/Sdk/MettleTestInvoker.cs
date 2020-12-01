using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Xunit.Sdk
{
    /// <summary>
    /// The test invoker for xUnit.net v2 tests.
    /// </summary>
    public class MettleTestInvoker : TestInvoker<IXunitTestCase>
    {
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes;

        private readonly Stack<BeforeAfterTestAttribute> beforeAfterAttributesRun = new Stack<BeforeAfterTestAttribute>();

        private IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MettleTestInvoker"/> class.
        /// </summary>
        /// <param name="test">The test that this invocation belongs to.</param>
        /// <param name="messageBus">The message bus to report run status to.</param>
        /// <param name="testClass">The test class that the test method belongs to.</param>
        /// <param name="constructorArguments">The arguments to be passed to the test class constructor.</param>
        /// <param name="testMethod">The test method that will be invoked.</param>
        /// <param name="testMethodArguments">The arguments to be passed to the test method.</param>
        /// <param name="beforeAfterAttributes">The list of <see cref="BeforeAfterTestAttribute"/>s for this test invocation.</param>
        /// <param name="aggregator">The exception aggregator used to run code and collect exceptions.</param>
        /// <param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
        public MettleTestInvoker(ITest test,
                                IMessageBus messageBus,
                                Type testClass,
                                object[] constructorArguments,
                                MethodInfo testMethod,
                                object[] testMethodArguments,
                                IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
                                ExceptionAggregator aggregator,
                                CancellationTokenSource cancellationTokenSource,
                                IServiceProvider serviceProvider)
            : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, aggregator, cancellationTokenSource)
        {
            this.beforeAfterAttributes = beforeAfterAttributes;
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates the test class, unless the test method is static or there have already been errors. Note that
        /// this method times the creation of the test class (using <see cref="Timer"/>). It is also responsible for
        /// sending the <see cref="ITestClassConstructionStarting"/>and <see cref="ITestClassConstructionFinished"/>
        /// messages, so if you override this method without calling the base, you are responsible for all of this behavior.
        /// This method should NEVER throw; any exceptions should be placed into the <see cref="Aggregator"/>.
        /// </summary>
        /// <returns>The class instance, if appropriate; <c>null</c>, otherwise</returns>
        protected override object CreateTestClass()
        {
            object testClass = null;

            if (!TestMethod.IsStatic && !Aggregator.HasExceptions)
                testClass = Test.CreateTestClass(TestClass, ConstructorArguments, MessageBus, Timer, CancellationTokenSource);

            return testClass;
        }

        /// <summary>
        /// Gets the list of <see cref="BeforeAfterTestAttribute"/>s for this test invocation.
        /// </summary>
        protected IReadOnlyList<BeforeAfterTestAttribute> BeforeAfterAttributes
            => beforeAfterAttributes;

        /// <inheritdoc/>
        protected override Task BeforeTestMethodInvokedAsync()
        {
            foreach (var beforeAfterAttribute in beforeAfterAttributes)
            {
                var attributeName = beforeAfterAttribute.GetType().Name;
                if (!MessageBus.QueueMessage(new BeforeTestStarting(Test, attributeName)))
                {
                    CancellationTokenSource.Cancel();
                }
                else
                {
                    try
                    {
                        Timer.Aggregate(() => beforeAfterAttribute.Before(TestMethod));
                        beforeAfterAttributesRun.Push(beforeAfterAttribute);
                    }
                    catch (Exception ex)
                    {
                        Aggregator.Add(ex);
                        break;
                    }
                    finally
                    {
                        if (!MessageBus.QueueMessage(new BeforeTestFinished(Test, attributeName)))
                            CancellationTokenSource.Cancel();
                    }
                }

                if (CancellationTokenSource.IsCancellationRequested)
                    break;
            }

            return CommonTasks.Completed;
        }

        /// <inheritdoc/>
        protected override Task AfterTestMethodInvokedAsync()
        {
            foreach (var beforeAfterAttribute in beforeAfterAttributesRun)
            {
                var attributeName = beforeAfterAttribute.GetType().Name;
                if (!MessageBus.QueueMessage(new AfterTestStarting(Test, attributeName)))
                    CancellationTokenSource.Cancel();

                Aggregator.Run(() => Timer.Aggregate(() => beforeAfterAttribute.After(TestMethod)));

                if (!MessageBus.QueueMessage(new AfterTestFinished(Test, attributeName)))
                    CancellationTokenSource.Cancel();
            }

            return CommonTasks.Completed;
        }

        /// <inheritdoc/>
        protected override Task<decimal> InvokeTestMethodAsync(object testClassInstance)
        {
            if (TestCase.InitializationException != null)
            {
                var tcs = new TaskCompletionSource<decimal>();
                tcs.SetException(TestCase.InitializationException);
                return tcs.Task;
            }

            return TestCase.Timeout > 0
                ? InvokeTimeoutTestMethodAsync(testClassInstance)
                : base.InvokeTestMethodAsync(testClassInstance);
        }

        private async Task<decimal> InvokeTimeoutTestMethodAsync(object testClassInstance)
        {
            var baseTask = base.InvokeTestMethodAsync(testClassInstance);
            var resultTask = await Task.WhenAny(baseTask, Task.Delay(TestCase.Timeout)).ConfigureAwait(false);

            if (resultTask != baseTask)
                throw new TestTimeoutException(TestCase.Timeout);

            return baseTask.Result;
        }
    }
}
