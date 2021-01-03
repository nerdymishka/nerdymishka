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
    public class Dotnet : ShellExecutor
    {
        public Dotnet()
            : base()
        {
            this.Setup();
        }

        public Dotnet(ShellExecutor executor, ILogger logger = null)
            : base(executor, logger)
        {
            this.Setup();
        }

        public ShellResult InstallTool(string name, bool global)
        {
            var flags = string.Empty;
            if (global)
                flags += " -g";

            return this.Exec($"tool install {name}{flags}");
        }

        protected void Setup()
        {
            var path = Environment.GetEnvironmentVariable("NMX_DOTNET_HOME");
            var exe = OperatingSystem.IsWindows() ? "dotnet.exe" : "dotnet";
            if (!string.IsNullOrWhiteSpace(path))
            {
                this.SetBaseCommand(Path.Combine(path, exe));
                return;
            }

            this.SetBaseCommand(exe);
        }

        public class TestOptions : CommandOptions
        {
            public string Target
            {
                get => this.Get<string>("{target}");
                set => this.Set("{target}", value);
            }

            public string Filter
            {
                get => this.Get<string>("--filter");
                set => this.Set("--filter", value);
            }

            public string Configuration
            {
                get => this.Get<string>("--configuration");
                set => this.Set("--configuration", value);
            }

            public bool NoRestore
            {
                get => this.Get<bool>("--no-restore");
                set => this.Set("--no-restore", value);
            }

            public bool Blame
            {
                get => this.Get<bool>("--blame");
                set => this.Set("--blame", value);
            }

            public bool BlameCrash
            {
                get => this.Get<bool>("--blame-crash");
                set => this.Set("--blame-crash", value);
            }

            public string Settings
            {
                get => this.Get<string>("--settings");
                set => this.Set("--settings", value);
            }

            public bool NoLogo { get; set; }
        }
    }
}
