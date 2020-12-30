using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Diagnostics
{
    public static class Shell
    {
        public static ShellCaptureResult Execute(string command)
        {
            Check.ArgNotWhiteSpace(command, nameof(command));

            if (command.Contains(' '))
            {
                var firstSpaceIndex = command.IndexOf(' ');
                var fileName = command.Substring(0, firstSpaceIndex);
                var args = command.Substring(firstSpaceIndex + 1);

                return Execute(fileName, args);
            }

            return Execute(command, (string)null);
        }

        public static ShellCaptureResult Execute(
            string fileName,
            string arguments)
        {
            return Execute(fileName, arguments, (string)null, null);
        }

        public static ShellCaptureResult Execute(
            string fileName,
            string arguments,
            string workingDirectory,
            int? timeout = null)
        {
            var info = new ProcessStartInfo(fileName);
            if (!string.IsNullOrWhiteSpace(arguments))
                info.Arguments = arguments;

            if (!string.IsNullOrWhiteSpace(workingDirectory))
                info.WorkingDirectory = workingDirectory;

            return Execute(
                info,
                timeout);
        }

        public static ShellCaptureResult Execute(
            ProcessStartInfo info,
            int? timeout = null)
        {
            Check.ArgNotNull(info, nameof(info));

            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            var stdOut = new List<string>();
            var stdError = new List<string>();
            void OutHandler(object sender, DataReceivedEventArgs args)
            {
                stdOut.Add(args.Data);
            }

            void ErrorHandler(object sender, DataReceivedEventArgs args)
            {
                stdError.Add(args.Data);
            }

            return Execute(
                info,
                (process) => new ShellCaptureResult()
                {
                    Arguments = info.Arguments,
                    FileName = info.FileName,
                    StartedAt = process.StartTime,
                    Duration = process.ExitTime - process.StartTime,
                    ExitCode = process.ExitCode,
                    StdOut = stdOut,
                    StdError = stdError,
                },
                outHandler: OutHandler,
                errorHandler: ErrorHandler,
                timeout);
        }

        public static ShellResult Execute(
            string fileName,
            string arguments = null,
            string workingDirectory = null,
            DataReceivedEventHandler outHandler = null,
            DataReceivedEventHandler errorHandler = null,
            int? timeout = null)
        {
            var info = new ProcessStartInfo(fileName);
            if (!string.IsNullOrWhiteSpace(arguments))
                info.Arguments = arguments;

            if (!string.IsNullOrWhiteSpace(workingDirectory))
                info.WorkingDirectory = workingDirectory;

            info.RedirectStandardError = errorHandler != null;
            info.RedirectStandardOutput = outHandler != null;

            return Execute(
                info,
                (process) => new ShellResult()
                {
                    Arguments = info.Arguments,
                    FileName = info.FileName,
                    StartedAt = process.StartTime,
                    Duration = process.ExitTime - process.StartTime,
                    ExitCode = process.ExitCode,
                },
                outHandler,
                errorHandler,
                timeout);
        }

        public static ShellResult Execute(
            ProcessStartInfo info,
            DataReceivedEventHandler outHandler = null,
            DataReceivedEventHandler errorHandler = null,
            int? timeout = null)
        {
            Check.ArgNotNull(info, nameof(info));

            info.RedirectStandardError = errorHandler != null;
            info.RedirectStandardOutput = outHandler != null;

            return Execute(
                info,
                (process) => new ShellResult()
                {
                    Arguments = info.Arguments,
                    FileName = info.FileName,
                    StartedAt = process.StartTime,
                    Duration = process.ExitTime - process.StartTime,
                    ExitCode = process.ExitCode,
                },
                outHandler,
                errorHandler,
                timeout);
        }

        public static T Execute<T>(
            ProcessStartInfo info,
            Func<Process, T> map,
            DataReceivedEventHandler outHandler = null,
            DataReceivedEventHandler errorHandler = null,
            int? timeout = null)
        {
            Check.ArgNotNull(info, nameof(info));
            Check.ArgNotNull(map, nameof(map));

            using var process = new Process()
            {
                StartInfo = info,
            };

            var redirect = info.RedirectStandardError || info.RedirectStandardOutput;
            if (redirect)
            {
                info.UseShellExecute = false;
                info.CreateNoWindow = true;
            }

            if (info.RedirectStandardOutput && outHandler != null)
                process.OutputDataReceived += outHandler;

            if (info.RedirectStandardError && errorHandler != null)
                process.ErrorDataReceived += errorHandler;

            process.Start();

            if (redirect)
            {
                if (info.RedirectStandardError)
                    process.BeginErrorReadLine();

                if (info.RedirectStandardOutput)
                    process.BeginOutputReadLine();
            }

            if (timeout > 0)
                process.WaitForExit(timeout.Value);
            else
                process.WaitForExit();

            return map(process);
        }

        public static async Task<ShellCaptureResult> ExecuteAsync(
            string fileName,
            string arguments = null,
            string workingDirectory = null,
            CancellationToken token = default)
        {
            var info = new ProcessStartInfo(fileName);
            if (!string.IsNullOrWhiteSpace(arguments))
                info.Arguments = arguments;

            if (!string.IsNullOrWhiteSpace(workingDirectory))
                info.WorkingDirectory = workingDirectory;

            return await ExecuteAsync(
                    info,
                    token)
                .ConfigureAwait(false);
        }

        public static async Task<ShellResult> ExecuteAsync(
            string fileName,
            string arguments = null,
            string workingDirectory = null,
            DataReceivedEventHandler outHandler = null,
            DataReceivedEventHandler errorHandler = null,
            CancellationToken token = default)
        {
            var info = new ProcessStartInfo(fileName);
            if (!string.IsNullOrWhiteSpace(arguments))
                info.Arguments = arguments;

            if (!string.IsNullOrWhiteSpace(workingDirectory))
                info.WorkingDirectory = workingDirectory;

            info.RedirectStandardError = errorHandler != null;
            info.RedirectStandardOutput = outHandler != null;

            return await ExecuteAsync(
                    info,
                    (process) => new ShellResult()
                    {
                        Arguments = info.Arguments,
                        FileName = info.FileName,
                        StartedAt = process.StartTime,
                        Duration = process.ExitTime - process.StartTime,
                        ExitCode = process.ExitCode,
                    },
                    outHandler,
                    errorHandler,
                    token)
                .ConfigureAwait(false);
        }

        public static async Task<ShellCaptureResult> ExecuteAsync(
            ProcessStartInfo info,
            CancellationToken token = default)
        {
            Check.ArgNotNull(info, nameof(info));

            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            var stdOut = new List<string>();
            var stdError = new List<string>();
            void OutHandler(object sender, DataReceivedEventArgs args)
            {
                stdOut.Add(args.Data);
            }

            void ErrorHandler(object sender, DataReceivedEventArgs args)
            {
                stdError.Add(args.Data);
            }

            return await ExecuteAsync(
                    info,
                    (process) => new ShellCaptureResult()
                    {
                        Arguments = info.Arguments,
                        FileName = info.FileName,
                        StartedAt = process.StartTime,
                        Duration = process.ExitTime - process.StartTime,
                        ExitCode = process.ExitCode,
                        StdOut = stdOut,
                        StdError = stdError,
                    },
                    outHandler: OutHandler,
                    errorHandler: ErrorHandler,
                    token: token)
                .ConfigureAwait(false);
        }

        public static async Task<ShellResult> ExecuteAsync(
            ProcessStartInfo info,
            DataReceivedEventHandler outHandler = null,
            DataReceivedEventHandler errorHandler = null,
            CancellationToken token = default)
        {
            Check.ArgNotNull(info, nameof(info));

            info.RedirectStandardError = errorHandler != null;
            info.RedirectStandardOutput = outHandler != null;

            return await ExecuteAsync(
                    info,
                    (process) => new ShellResult()
                    {
                        Arguments = info.Arguments,
                        FileName = info.FileName,
                        StartedAt = process.StartTime,
                        Duration = process.ExitTime - process.StartTime,
                        ExitCode = process.ExitCode,
                    },
                    outHandler,
                    errorHandler,
                    token)
                .ConfigureAwait(false);
        }

        public static async Task<T> ExecuteAsync<T>(
            ProcessStartInfo info,
            Func<Process, T> map,
            DataReceivedEventHandler outHandler = null,
            DataReceivedEventHandler errorHandler = null,
            CancellationToken token = default)
        {
            Check.ArgNotNull(info, nameof(info));
            Check.ArgNotNull(map, nameof(map));

            using var process = new Process()
            {
                StartInfo = info,
            };

            var redirect = info.RedirectStandardError || info.RedirectStandardOutput;
            if (redirect)
            {
                info.UseShellExecute = false;
                info.CreateNoWindow = true;
            }

            if (info.RedirectStandardOutput && outHandler != null)
                process.OutputDataReceived += outHandler;

            if (info.RedirectStandardError && errorHandler != null)
                process.ErrorDataReceived += errorHandler;

            if (redirect)
            {
                if (info.RedirectStandardError)
                    process.BeginErrorReadLine();

                if (info.RedirectStandardOutput)
                    process.BeginOutputReadLine();
            }

            await process.WaitForExitAsync(token)
                .ConfigureAwait(false);

            return map(process);
        }
    }
}
