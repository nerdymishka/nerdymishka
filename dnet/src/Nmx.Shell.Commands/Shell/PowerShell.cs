using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NerdyMishka.Diagnostics;

namespace NerdyMishka.Shell
{
    public class PowerShell : ShellExecutor
    {
        public PowerShell()
            : base()
        {
            this.Setup();
        }

        public PowerShell(ShellExecutor executor, ILogger logger = null)
           : base(executor, logger)
        {
            this.Setup();
        }

        public ShellResult ExecCommand(string command, bool noProfile = true)
        {
            var profileFlag = noProfile ? "-NoProfile " : string.Empty;
            var bytes = System.Text.Encoding.Unicode.GetBytes(command);
            var base64 = Convert.ToBase64String(bytes);
            var args = $" -NoLogo -NonInteractive {profileFlag}-ExecutionPolicy ByPass -EncodedCommand {base64}";

            return this.Exec(args);
        }

        public ShellResult ExecFile(string file, bool noProfile)
        {
            Check.ArgNotNull(file, nameof(file));
            Check.ArgNotWhiteSpace(file, nameof(file));

            if (!File.Exists(file))
                throw new FileNotFoundException(file);

            var profileFlag = noProfile ? "-NoProfile " : string.Empty;
            var args = $" -NoLogo -NonInteractive {profileFlag}-ExecutionPolicy ByPass -File {file}";

            return this.Exec(args);
        }

        protected void Setup()
        {
            if (IsCommandAvailable("pwsh"))
            {
                this.SetBaseCommand("pwsh");
                return;
            }

            if (OperatingSystem.IsWindows())
            {
                this.SetBaseCommand("powershell");
            }
            else
            {
                this.SetBaseCommand("pwsh");
            }
        }
    }
}
