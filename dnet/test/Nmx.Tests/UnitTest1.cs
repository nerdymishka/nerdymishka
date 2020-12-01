using System;
using System.Diagnostics.CodeAnalysis;
using Mettle;

namespace Tests
{
    public class UnitTest1
    {
        [UnitTest]
        public void Test1([NotNull] IAssert assert)
        {
            assert.True("x".Equals("x"));
        }
    }
}
