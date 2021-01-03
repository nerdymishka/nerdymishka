using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NerdyMishka.Diagnostics;
using static NerdyMishka.Diagnostics.Shell;

namespace NerdyMishka.Shell
{
    public class ShellExecutor
    {
        private string baseCommand;
        private ILogger logger;
        private bool? baseCommandFound;

        public ShellExecutor()
        {
        }

        public ShellExecutor(string baseCommand, ShellExecutor executor, ILogger logger = null)
            : this(executor, logger)
        {
            this.BaseCommand = baseCommand;
        }

        public ShellExecutor(ShellExecutor executor, ILogger logger = null)
        {
            Check.ArgNotNull(executor, nameof(executor));
            this.Timeout = executor.Timeout;
            this.WorkingDirectory = executor.WorkingDirectory;
            this.StdErrorHandler = executor.StdErrorHandler;
            this.StdOutHandler = executor.StdOutHandler;
            this.logger = logger;
        }

        public string WorkingDirectory { get; set; }

        public int? Timeout { get; set; }

        public DataReceivedEventHandler StdOutHandler { get; set; }

        public DataReceivedEventHandler StdErrorHandler { get; set; }

        public string BaseCommand
        {
            get => this.baseCommand;
            private set
            {
                this.baseCommand = value;
                this.HasBaseCommand = !string.IsNullOrWhiteSpace(this.baseCommand);
            }
        }

        public bool HasBaseCommand { get; private set; }

        public bool IsBaseCommandAvailable
        {
            get
            {
                if (!this.HasBaseCommand)
                    return false;

                if (this.baseCommandFound.HasValue)
                    return this.baseCommandFound.Value;

                if (this.baseCommand.Contains(Path.DirectorySeparatorChar) ||
                    this.baseCommand.Contains(Path.AltDirectorySeparatorChar))
                {
                    this.baseCommandFound = File.Exists(this.baseCommand);
                    return this.baseCommandFound.Value;
                }

                this.baseCommandFound = IsCommandAvailable(this.baseCommand);

                return this.baseCommandFound.Value;
            }
        }

        public static bool IsCommandAvailable(string command)
        {
            if (OperatingSystem.IsWindows())
            {
                return Execute($"where {command}").ExitCode == 0;
            }

            return Execute($"which {command}").ExitCode == 0;
        }

        public ShellExecutor SetBaseCommand(string command)
        {
            this.BaseCommand = command;
            return this;
        }

        public ShellExecutor SetWorkingDirectory(string workingDirectory)
        {
            Check.ArgNotWhiteSpace(workingDirectory, nameof(workingDirectory));

            workingDirectory = Path.GetFullPath(workingDirectory);
            this.WorkingDirectory = workingDirectory;

            return this;
        }

        public ShellExecutor SetStdOutHandler(DataReceivedEventHandler handler)
        {
            this.StdOutHandler = handler;

            return this;
        }

        public ShellExecutor SetStdErrorHandler(DataReceivedEventHandler handler)
        {
            this.StdErrorHandler = handler;

            return this;
        }

        public ShellExecutor SetTimeout(int? timeout)
        {
            this.Timeout = timeout;
            return this;
        }

        public bool TryExec(ReadOnlySpan<char> command, out ShellResult result)
        {
            if (this.HasBaseCommand)
            {
                if (!this.IsBaseCommandAvailable)
                    throw new NotSupportedException($"Executable '{this.BaseCommand}' was not found.");

                var sb = new StringBuilder();
                sb.Append(this.baseCommand);
                if (!command.IsEmpty)
                    sb.Append(' ').Append(command);

                command = sb.ToString();
            }

            string arguments = null;

            if (command.Contains(' '))
            {
                var firstSpaceIndex = command.IndexOf(' ');
                arguments = command.Slice(firstSpaceIndex + 1).ToString();
                command = command.Slice(0, firstSpaceIndex);
            }

            var cmd = command.ToString();

            result = new ShellResult()
            {
                ExitCode = -1,
                Arguments = arguments,
                FileName = cmd,
            };

            if (IsCommandAvailable(cmd))
            {
                if (this.logger is not null && this.logger.IsEnabled(LogLevel.Debug))
                {
                    this.logger?.LogDebug($"{cmd} {arguments}");
                }

                if (this.StdOutHandler != null || this.StdErrorHandler != null)
                {
                    result = Execute(cmd,
                        arguments,
                        this.WorkingDirectory,
                        this.StdOutHandler,
                        this.StdErrorHandler,
                        this.Timeout);

                    return true;
                }

                result = Execute(cmd, arguments, this.WorkingDirectory, this.Timeout);
                return true;
            }

            return false;
        }

        public ShellResult Exec(ReadOnlySpan<char> command)
        {
            if (this.HasBaseCommand)
            {
                if (!this.IsBaseCommandAvailable)
                    throw new NotSupportedException($"Executable '{this.BaseCommand}' was not found.");

                var sb = new StringBuilder();
                sb.Append(this.baseCommand);
                if (!command.IsEmpty)
                    sb.Append(' ').Append(command);

                command = sb.ToString();
            }

            string arguments = null;

            if (command.Contains(' '))
            {
                var firstSpaceIndex = command.IndexOf(' ');
                arguments = command.Slice(firstSpaceIndex + 1).ToString();
                command = command.Slice(0, firstSpaceIndex);
            }

            var cmd = command.ToString();

            if (this.logger is not null && this.logger.IsEnabled(LogLevel.Debug))
            {
                this.logger?.LogDebug($"{cmd} {arguments}");
            }

            if (this.StdOutHandler != null || this.StdErrorHandler != null)
            {
                return Execute(cmd,
                    arguments,
                    this.WorkingDirectory,
                    this.StdOutHandler,
                    this.StdErrorHandler,
                    this.Timeout);
            }

            return Execute(cmd, arguments, this.WorkingDirectory, this.Timeout);
        }
    }
}
