using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mettle;
using NerdyMishka.Diagnostics;
using NerdyMishka.Shell;

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

        [Functional]
        public static void PowerShellHelloWorld([NotNull] IAssert assert)
        {
            var pwsh = new PowerShell();
            var result = (ShellCaptureResult)pwsh.ExecCommand("Write-Host 'Hello World'");
            assert.NotNull(result);
            assert.Equal(0, result.ExitCode);
            var firstLine = result.StdOut.FirstOrDefault();
            assert.NotEmpty(firstLine);
            assert.True(firstLine.Contains("Hello World"));
        }
    }
}
