using System;
using System.CommandLine.Invocation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace NerdyMishka.CommandLine.Hosting
{
    public class InvocationLifetime : IHostLifetime
    {
        private readonly CancellationToken invokeCancelToken;

        private CancellationTokenRegistration appStartedReg;

        private CancellationTokenRegistration appStoppingReg;

        private CancellationTokenRegistration invokeCancelReg;

        public InvocationLifetime(
            IOptions<InvocationLifetimeOptions> options,
            IHostEnvironment environment,
            IHostApplicationLifetime applicationLifetime,
            InvocationContext context = null,
            ILoggerFactory loggerFactory = null)
        {
            this.Options = options?.Value ?? throw new ArgumentNullException(nameof(options));

            this.Environment = environment
                ?? throw new ArgumentNullException(nameof(environment));

            this.ApplicationLifetime = applicationLifetime
                ?? throw new ArgumentNullException(nameof(applicationLifetime));

            // if InvocationLifetime is added outside of a System.CommandLine
            // invocation pipeline context will be null.
            // Use default cancellation token instead, and become a noop lifetime.
            this.invokeCancelToken = context?.GetCancellationToken() ?? default;

            this.Logger = (loggerFactory ?? NullLoggerFactory.Instance)
                .CreateLogger("Microsoft.Hosting.Lifetime");
        }

        public InvocationLifetimeOptions Options { get; }

        public IHostEnvironment Environment { get; }

        public IHostApplicationLifetime ApplicationLifetime { get; }

        private ILogger Logger { get; }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            if (!this.Options.SuppressStatusMessages)
            {
                this.appStartedReg = this.ApplicationLifetime.ApplicationStarted.Register(
                    state => { ((InvocationLifetime)state).OnApplicationStarted(); }, this);

                this.appStoppingReg = this.ApplicationLifetime.ApplicationStopping.Register(
                    state => { ((InvocationLifetime)state).OnApplicationStopping(); }, this);
            }

            this.invokeCancelReg =
                this.invokeCancelToken.Register(state => { ((InvocationLifetime)state).OnInvocationCancelled(); },
                    this);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // There's nothing to do here
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this.invokeCancelReg.Dispose();
            this.appStartedReg.Dispose();
            this.appStoppingReg.Dispose();
        }

        private void OnInvocationCancelled()
        {
            this.ApplicationLifetime.StopApplication();
        }

        private void OnApplicationStarted()
        {
            this.Logger.LogInformation("Application started. Press Ctrl+C to shut down.");
            this.Logger.LogInformation("Hosting environment: {envName}", this.Environment.EnvironmentName);
            this.Logger.LogInformation("Content root path: {contentRoot}", this.Environment.ContentRootPath);
        }

        private void OnApplicationStopping()
        {
            this.Logger.LogInformation("Application is shutting down...");
        }
    }
}