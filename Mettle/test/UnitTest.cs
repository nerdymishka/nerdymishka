using System;
using System.Diagnostics.CodeAnalysis;
using Mettle;

namespace Tests
{
    [SuppressMessage("", "IDE0052:", Justification = "To Show what is possible")]
    public class UnitTest
    {
        private readonly IAssert assert;

        public UnitTest(IAssert assert)
        {
            this.assert = assert;
        }

        [UnitTest]
        public void Test1(IAssert assert)
        {
            if (assert == null)
                throw new ArgumentNullException(nameof(assert));

            assert.Ok("a" == "a");
            Console.Write("Test");
        }

        [UnitTest]
        [ServiceProviderFactory(typeof(ServiceBuilder2))]
        public void Test3(UnitTestData data)
        {
            var assert = AssertImpl.Current;
            assert.NotNull(data);
        }

        [Xunit.Fact]
        public void Test2()
        {
            Console.Write("Test");
        }
    }
}
