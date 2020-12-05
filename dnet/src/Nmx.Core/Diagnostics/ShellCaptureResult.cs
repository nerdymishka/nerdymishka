using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Diagnostics
{
    public record ShellCaptureResult : ShellResult
    {
        public IReadOnlyList<string> StdOut { get; init; }

        public IReadOnlyList<string> StdError { get; init; }
    }
}
