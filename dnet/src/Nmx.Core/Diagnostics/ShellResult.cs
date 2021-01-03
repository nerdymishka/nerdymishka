using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Diagnostics
{
    public record ShellResult
    {
        public string FileName { get; init; }

        public string Arguments { get; init; }

        public int ExitCode { get; init; }

        public DateTimeOffset? StartedAt { get; init; }

        public TimeSpan Duration { get; init; }
    }
}
