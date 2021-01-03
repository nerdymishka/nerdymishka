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
    public class SevenZip : ShellExecutor
    {
        public SevenZip()
            : base()
        {
        }

        public SevenZip(ShellExecutor executor, ILogger logger = null)
           : base(executor, logger)
        {
            if (OperatingSystem.IsWindows())
            {
                this.SetBaseCommand("7za");
            }
            else
            {
                this.SetBaseCommand("7z");
            }
        }

        public ShellResult Extract(string package, string destination, bool fullPath = false)
        {
            Check.ArgNotNull(package, nameof(package));
            Check.ArgNotWhiteSpace(package, nameof(package));

            if (package.Contains(Path.DirectorySeparatorChar) ||
                package.Contains(Path.AltDirectorySeparatorChar))
            {
                package = Path.GetFullPath(package);
            }

            destination = Path.GetFullPath(destination);
            var extract = fullPath ? 'x' : 'e';
            return this.Exec($"{extract} \"{package}\" -o\"{destination}\"");
        }

        public ShellResult Archive(string package, string glob, bool overwrite = true)
        {
            Check.ArgNotNull(package, nameof(package));
            Check.ArgNotWhiteSpace(package, nameof(package));

            if (package.Contains(Path.DirectorySeparatorChar) ||
                package.Contains(Path.AltDirectorySeparatorChar))
            {
                package = Path.GetFullPath(package);
            }

            var archive = overwrite ? "aoa" : "a";
            var ext = Path.GetExtension(package);
            var type = ext.Trim('.');

            return this.Exec($"{archive} -t{type} \"{package}\" {glob}");
        }

        protected void Setup()
        {
            var path = Environment.GetEnvironmentVariable("NMX_7ZIP_HOME");
            var exe = OperatingSystem.IsWindows() ? "7za.exe" : "7z";

            if (!string.IsNullOrWhiteSpace(path))
            {
                this.SetBaseCommand(Path.Combine(path, exe));
                return;
            }

            this.SetBaseCommand(exe);
        }
    }
}
