using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Xunit.Sdk
{
    /// <summary>
    /// The test class runner for xUnit.net v2 tests.
    /// </summary>
    public class MettleTestClassRunner : TestClassRunner<IXunitTestCase>
    {
        private readonly IDictionary<Type, object> collectionFixtureMappings;
        private IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MettleTestClassRunner"/> class.
        /// </summary>
        /// <param name="testClass">The test class to be run.</param>
        /// <param name="class">The test class that contains the tests to be run.</param>
        /// <param name="testCases">The test cases to be run.</param>
        /// <param name="diagnosticMessageSink">The message sink used to send diagnostic messages</param>
        /// <param name="messageBus">The message bus to report run status to.</param>
        /// <param name="testCaseOrderer">The test case orderer that will be used to decide how to order the test.</param>
        /// <param name="aggregator">The exception aggregator used to run code and collect exceptions.</param>
        /// <param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
        /// <param name="collectionFixtureMappings">The mapping of collection fixture types to fixtures.</param>
        public MettleTestClassRunner(ITestClass testClass,
                                    IReflectionTypeInfo @class,
                                    IEnumerable<IXunitTestCase> testCases,
                                    IMessageSink diagnosticMessageSink,
                                    IMessageBus messageBus,
                                    ITestCaseOrderer testCaseOrderer,
                                    ExceptionAggregator aggregator,
                                    CancellationTokenSource cancellationTokenSource,
                                    IDictionary<Type, object> collectionFixtureMappings,
                                    IServiceProvider serviceProvider)
            : base(testClass, @class, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
        {
            this.collectionFixtureMappings = collectionFixtureMappings;
            this.serviceProvider = serviceProvider;

            if (this.serviceProvider == null)
            {
#pragma warning disable CA1062 // Validate arguments of public methods
                var serviceProviderFactoryAttribute = testClass.Class.Assembly
#pragma warning restore CA1062 // Validate arguments of public methods
                        .GetCustomAttributes(typeof(ServiceProviderFactoryAttribute))
                        .SingleOrDefault();

                if (serviceProviderFactoryAttribute != null)
                {
                    var factoryType = serviceProviderFactoryAttribute.GetNamedArgument<Type>("FactoryType");

                    // TODO: consider diagnostic message if the type exists but not the interface.
                    if (factoryType != null && factoryType.GetInterface("IServiceProviderFactory") != null)
                    {
                        var serviceFactory = (IServiceProviderFactory)Activator.CreateInstance(factoryType);
                        this.serviceProvider = serviceFactory.CreateProvider();
                    }
                }

                this.serviceProvider = this.serviceProvider ?? new SimpleServiceProvider();
            }
        }

        /// <summary>
        /// Gets the fixture mappings that were created during <see cref="AfterTestClassStartingAsync"/>.
        /// </summary>
        protected virtual Dictionary<Type, object> ClassFixtureMappings { get; private set; } = new Dictionary<Type, object>();

        /// <summary>
        /// Gets the already initialized async fixtures <see cref="CreateClassFixtureAsync"/>.
        /// </summary>
        protected virtual HashSet<IAsyncLifetime> InitializedAsyncFixtures { get; private set; } = new HashSet<IAsyncLifetime>();

        /// <summary>
        /// Creates the arguments for the test class constructor. Attempts to resolve each parameter
        /// individually, and adds an error when the constructor arguments cannot all be provided.
        /// If the class is static, does not look for constructor, since one will not be needed.
        /// </summary>
        /// <returns>The test class constructor arguments.</returns>
        protected override object[] CreateTestClassConstructorArguments()
        {
            Console.WriteLine("Create Test Called");
            var isStaticClass = Class.Type.GetTypeInfo().IsAbstract && Class.Type.GetTypeInfo().IsSealed;
            if (!isStaticClass)
            {
                var ctor = SelectTestClassConstructor();
                if (ctor != null)
                {
                    var unusedArguments = new List<Tuple<int, ParameterInfo>>();
                    var parameters = ctor.GetParameters();

                    object[] constructorArguments = new object[parameters.Length];
                    for (int idx = 0; idx < parameters.Length; ++idx)
                    {
                        var parameter = parameters[idx];

                        if (TryGetConstructorArgument(ctor, idx, parameter, out object argumentValue))
                            constructorArguments[idx] = argumentValue;
                        else if (parameter.HasDefaultValue)
                            constructorArguments[idx] = parameter.DefaultValue;
                        else if (parameter.IsOptional)
                            constructorArguments[idx] = parameter.ParameterType.GetTypeInfo().GetDefaultValue();
                        else if (parameter.GetCustomAttribute<ParamArrayAttribute>() != null)
                            constructorArguments[idx] = Array.CreateInstance(parameter.ParameterType, 0);
                        else
                            unusedArguments.Add(Tuple.Create(idx, parameter));
                    }

                    if (unusedArguments.Count > 0)
                        Aggregator.Add(new TestClassException(FormatConstructorArgsMissingMessage(ctor, unusedArguments)));

                    return constructorArguments;
                }
            }

            return Array.Empty<object>();
        }

        /// <summary>
        /// Creates the instance of a class fixture type to be used by the test class. If the fixture can be created,
        /// it should be placed into the <see cref="ClassFixtureMappings"/> dictionary; if it cannot, then the method
        /// should record the error by calling <code>Aggregator.Add</code>.
        /// </summary>
        /// <param name="fixtureType">The type of the fixture to be created</param>
        protected virtual void CreateClassFixture(Type fixtureType)
        {
            if (fixtureType is null)
                throw new ArgumentNullException(nameof(fixtureType));

            var ctors = fixtureType.GetTypeInfo()
                                   .DeclaredConstructors
                                   .Where(ci => !ci.IsStatic && ci.IsPublic)
                                   .ToList();

            if (ctors.Count != 1)
            {
                Aggregator.Add(new TestClassException($"Class fixture type '{fixtureType.FullName}' may only define a single public constructor."));
                return;
            }

            var ctor = ctors[0];
            var missingParameters = new List<ParameterInfo>();
            var ctorArgs = ctor.GetParameters().Select(p =>
            {
                object arg;
                if (p.ParameterType == typeof(IMessageSink))
                    arg = DiagnosticMessageSink;
                else if (!collectionFixtureMappings.TryGetValue(p.ParameterType, out arg))
                    arg = this.serviceProvider?.GetService(p.ParameterType);

                if (arg == null)
                    missingParameters.Add(p);

                return arg;
            }).ToArray();

            if (missingParameters.Count > 0)
            {
                Aggregator.Add(new TestClassException(
                    $"Class fixture type '{fixtureType.FullName}' had one or more unresolved constructor arguments:"
                    + $" {string.Join(", ", missingParameters.Select(p => $"{p.ParameterType.Name} {p.Name}"))}"));
            }
            else
            {
                Aggregator.Run(() => ClassFixtureMappings[fixtureType] = ctor.Invoke(ctorArgs));
            }
        }

        private Task CreateClassFixtureAsync(Type fixtureType)
        {
            CreateClassFixture(fixtureType);
            var uninitializedFixtures = ClassFixtureMappings.Values
                                        .OfType<IAsyncLifetime>()
                                        .Where(fixture => !InitializedAsyncFixtures.Contains(fixture))
                                        .ToList();

            InitializedAsyncFixtures.UnionWith(uninitializedFixtures);
            return Task.WhenAll(uninitializedFixtures.Select(fixture => Aggregator.RunAsync(fixture.InitializeAsync)));
        }

        /// <inheritdoc/>
        protected override string FormatConstructorArgsMissingMessage(ConstructorInfo constructor, IReadOnlyList<Tuple<int, ParameterInfo>> unusedArguments)
            => $"The following constructor parameters did not have matching fixture data: {string.Join(", ", unusedArguments.Select(arg => $"{arg.Item2.ParameterType.Name} {arg.Item2.Name}"))}";

        /// <inheritdoc/>
        protected override async Task AfterTestClassStartingAsync()
        {
            var ordererAttribute = Class.GetCustomAttributes(typeof(TestCaseOrdererAttribute)).SingleOrDefault();
            if (ordererAttribute != null)
            {
                try
                {
                    var testCaseOrderer = ExtensibilityPointFactory.GetTestCaseOrderer(DiagnosticMessageSink, ordererAttribute);
                    if (testCaseOrderer != null)
                    {
                        TestCaseOrderer = testCaseOrderer;
                    }
                    else
                    {
                        var args = ordererAttribute.GetConstructorArguments().Cast<string>().ToList();
                        DiagnosticMessageSink.OnMessage(new DiagnosticMessage($"Could not find type '{args[0]}' in {args[1]} for class-level test case orderer on test class '{TestClass.Class.Name}'"));
                    }
                }
                catch (Exception ex)
                {
                    var innerEx = ex.Unwrap();
                    var args = ordererAttribute.GetConstructorArguments().Cast<string>().ToList();
                    DiagnosticMessageSink.OnMessage(new DiagnosticMessage($"Class-level test case orderer '{args[0]}' for test class '{TestClass.Class.Name}' threw '{innerEx.GetType().FullName}' during construction: {innerEx.Message}{Environment.NewLine}{innerEx.StackTrace}"));
                }
            }

            var testClassTypeInfo = Class.Type.GetTypeInfo();
            if (testClassTypeInfo.ImplementedInterfaces.Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollectionFixture<>)))
                Aggregator.Add(new TestClassException("A test class may not be decorated with ICollectionFixture<> (decorate the test collection class instead)."));

            var createClassFixtureAsyncTasks = new List<Task>();
            foreach (var interfaceType in testClassTypeInfo.ImplementedInterfaces.Where(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IClassFixture<>)))
                createClassFixtureAsyncTasks.Add(CreateClassFixtureAsync(interfaceType.GetTypeInfo().GenericTypeArguments.Single()));

            if (TestClass.TestCollection.CollectionDefinition != null)
            {
                var declarationType = ((IReflectionTypeInfo)TestClass.TestCollection.CollectionDefinition).Type;
                foreach (var interfaceType in declarationType.GetTypeInfo().ImplementedInterfaces.Where(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IClassFixture<>)))
                    createClassFixtureAsyncTasks.Add(CreateClassFixtureAsync(interfaceType.GetTypeInfo().GenericTypeArguments.Single()));
            }

            await Task.WhenAll(createClassFixtureAsyncTasks).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override async Task BeforeTestClassFinishedAsync()
        {
            var disposeAsyncTasks = ClassFixtureMappings.Values.OfType<IAsyncLifetime>().Select(fixture => Aggregator.RunAsync(fixture.DisposeAsync)).ToList();

            await Task.WhenAll(disposeAsyncTasks).ConfigureAwait(false);

            foreach (var fixture in ClassFixtureMappings.Values.OfType<IDisposable>())
                Aggregator.Run(fixture.Dispose);
        }

        /// <inheritdoc/>
        protected override Task<RunSummary> RunTestMethodAsync(ITestMethod testMethod, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, object[] constructorArguments)
            => new MettleTestMethodRunner(
                    testMethod, Class, method, testCases, DiagnosticMessageSink, MessageBus, new ExceptionAggregator(Aggregator), CancellationTokenSource, constructorArguments, this.serviceProvider).RunAsync();

        /// <inheritdoc/>
        protected override ConstructorInfo SelectTestClassConstructor()
        {
            var ctors = Class.Type.GetTypeInfo()
                                  .DeclaredConstructors
                                  .Where(ci => !ci.IsStatic && ci.IsPublic)
                                  .ToList();

            if (ctors.Count == 1)
                return ctors[0];

            Aggregator.Add(new TestClassException("A test class may only define a single public constructor."));
            return null;
        }

        /// <inheritdoc/>
        protected override bool TryGetConstructorArgument(ConstructorInfo constructor, int index, ParameterInfo parameter, out object argumentValue)
        {
            if (parameter is null)
                throw new ArgumentNullException(nameof(parameter));

            object svc = this.serviceProvider?.GetService(parameter.ParameterType);
            if (svc != null)
            {
                argumentValue = svc;
                return true;
            }

            if (parameter.ParameterType == typeof(ITestOutputHelper))
            {
                argumentValue = new TestOutputHelper();
                return true;
            }

            return ClassFixtureMappings.TryGetValue(parameter.ParameterType, out argumentValue)
                || collectionFixtureMappings.TryGetValue(parameter.ParameterType, out argumentValue);
        }
    }
}
