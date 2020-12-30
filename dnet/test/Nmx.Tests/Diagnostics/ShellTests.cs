using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mettle;
using NerdyMishka.Diagnostics;

namespace Tests.Diagnostics
{
    public class ShellTests
    {
        [Functional]
        public static void SimpleExecute([NotNull] IAssert assert)
        {
            var result = Shell.Execute("curl --version");
            assert.NotNull(result.StdOut);
        }
    }
}
