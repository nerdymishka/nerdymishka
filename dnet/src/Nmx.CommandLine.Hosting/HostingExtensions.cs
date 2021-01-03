using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace NerdyMishka.CommandLine.Hosting
{
    public static class HostingExtensions
    {
        private const string ConfigurationDirectiveName = "config";

        public static CommandLineBuilder UseHost(this CommandLineBuilder builder,
            Func<string[], IHostBuilder> hostBuilderFactory,
            Action<IHostBuilder> configureHost = null) =>
            builder.UseMiddleware(async (invocation, next) =>
            {
                var argsRemaining = invocation.ParseResult.UnparsedTokens.ToArray();
                var hostBuilder = hostBuilderFactory?.Invoke(argsRemaining)
                    ?? new HostBuilder();

                hostBuilder.Properties[typeof(InvocationContext)] = invocation;

                hostBuilder.ConfigureHostConfiguration(config =>
                {
                    config.AddCommandLineDirectives(invocation.ParseResult, ConfigurationDirectiveName);
                });

                hostBuilder.ConfigureServices(services =>
                {
                    services.AddSingleton(invocation);
                    services.AddSingleton(invocation.BindingContext);
                    services.AddSingleton(invocation.Console);
                    services.AddTransient(_ => invocation.InvocationResult);
                    services.AddTransient(_ => invocation.ParseResult);
                });
                hostBuilder.UseInvocationLifetime(invocation);
                configureHost?.Invoke(hostBuilder);

                using var host = hostBuilder.Build();

                invocation.BindingContext.AddService(typeof(IHost), _ => host);

                await host.StartAsync().ConfigureAwait(false);

                await next(invocation).ConfigureAwait(false);

                await host.StopAsync().ConfigureAwait(false);
            });

        public static IConfigurationBuilder AddCommandLineDirectives(
            this IConfigurationBuilder config,
            ParseResult commandline,
            string name)
        {
            if (commandline is null)
                throw new ArgumentNullException(nameof(commandline));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (!commandline.Directives.TryGetValues(name, out var directives))
                return config;

            var kvpSeparator = new[] { '=' };
            return config.AddInMemoryCollection(directives.Select(s =>
            {
                var parts = s.Split(kvpSeparator, count: 2);
                var key = parts[0];
                var value = parts.Length > 1 ? parts[1] : null;
                return new KeyValuePair<string, string>(key, value);
            }).ToList());
        }

        public static CommandLineBuilder UseHost(
            this CommandLineBuilder builder,
            Action<IHostBuilder> configureHost = null)
                => UseHost(builder, null, configureHost);

        public static IHostBuilder UseInvocationLifetime(this IHostBuilder host,
            InvocationContext invocation,
            Action<InvocationLifetimeOptions> configureOptions = null)
        {
            return host.ConfigureServices(services =>
            {
                services.TryAddSingleton(invocation);
                services.AddSingleton<IHostLifetime, InvocationLifetime>();
                if (configureOptions != null)
                    services.Configure(configureOptions);
            });
        }

        public static OptionsBuilder<TOptions> BindCommandLine<TOptions>(
            this OptionsBuilder<TOptions> optionsBuilder)
            where TOptions : class
        {
            if (optionsBuilder is null)
                throw new ArgumentNullException(nameof(optionsBuilder));

            return optionsBuilder.Configure<IServiceProvider>((opts, serviceProvider) =>
            {
                var modelBinder = serviceProvider.GetService<ModelBinder<TOptions>>()
                    ?? new ModelBinder<TOptions>();

                var bindingContext = serviceProvider.GetRequiredService<BindingContext>();
                modelBinder.UpdateInstance(opts, bindingContext);
            });
        }

        public static IHostBuilder UseCommandHandler<TCommand, THandler>(this IHostBuilder builder)
            where TCommand : Command
            where THandler : ICommandHandler
        {
            return builder.UseCommandHandler(typeof(TCommand), typeof(THandler));
        }

        public static IHostBuilder UseCommandHandler(this IHostBuilder builder, Type commandType, Type handlerType)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            if (handlerType is null)
                throw new ArgumentNullException(nameof(handlerType));

            if (!typeof(Command).IsAssignableFrom(commandType))
            {
                throw new ArgumentException($"{nameof(commandType)} must be a type of {nameof(Command)}", nameof(handlerType));
            }

            if (!typeof(ICommandHandler).IsAssignableFrom(handlerType))
            {
                throw new ArgumentException($"{nameof(handlerType)} must implement {nameof(ICommandHandler)}", nameof(handlerType));
            }

            if (builder.Properties[typeof(InvocationContext)] is InvocationContext invocation
                && invocation.ParseResult.CommandResult.Command is Command command
                && command.GetType() == commandType)
            {
                invocation.BindingContext.AddService(handlerType, c => c.GetService<IHost>().Services.GetService(handlerType));
                builder.ConfigureServices(services =>
                {
                    // the lastest version of System.CommandLine doesn't enable the BindingContext to bind to the
                    // value handler, so reflection is added below for ease of use to bind to properties.
                    services.AddTransient(handlerType, (s) =>
                    {
                        var ctor = handlerType.GetConstructors().SingleOrDefault();
                        object instance = null;
                        if (ctor != null)
                        {
                            var parmeterTypes = ctor.GetParameters();
                            var parameters = new List<object>();
                            foreach (var pt in parmeterTypes)
                            {
                                var value = s.GetService(pt.ParameterType);
                                parameters.Add(value);
                            }

                            instance = ctor.Invoke(parameters.ToArray());
                        }
                        else
                        {
                            instance = Activator.CreateInstance(handlerType);
                        }

                        var properties = handlerType.GetProperties().ToList();
                        var consoleProperty = properties.SingleOrDefault(o => o.PropertyType == typeof(IConsole));

                        if (consoleProperty != null)
                        {
                            var console = s.GetService<IConsole>();
                            if (console != null)
                                consoleProperty.SetValue(instance, console);
                        }

                        var context = s.GetService<InvocationContext>();

                        foreach (var o in context.ParseResult.CommandResult.Command.Options)
                        {
                            var name = o.Name.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase)
                                .ToUpperInvariant();

                            var value = context.ParseResult.ValueForOption(o.Aliases.First());

                            if (value is null)
                                continue;

                            var property = properties.SingleOrDefault(o => o.Name.ToUpperInvariant() == name);
                            if (property != null)
                                property.SetValue(instance, value);
                        }

                        foreach (var a in context.ParseResult.CommandResult.Command.Arguments)
                        {
                            var name = a.Name.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase)
                                .ToUpperInvariant();

                            var result = context.ParseResult.FindResultFor(a);
                            var value = result?.GetValueOrDefault();

                            if (value == null)
                                continue;

                            var property = properties.SingleOrDefault(o => o.Name.ToUpperInvariant() == name);
                            if (property != null)
                                property.SetValue(instance, value);
                        }

                        return instance;
                    });
                });

                var methodInfo = handlerType.GetMethod(nameof(ICommandHandler.InvokeAsync));
                if (methodInfo is null)
                    throw new NullReferenceException($"could not find method InvokeAsync on type {handlerType.FullName}");

                command.Handler = CommandHandler.Create(methodInfo);
            }

            return builder;
        }
    }
}