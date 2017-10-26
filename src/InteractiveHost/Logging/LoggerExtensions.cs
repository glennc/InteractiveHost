using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace InteractiveHost.Logging
{
    public static class ConsoleLoggerExtensions
    {
        /// <summary>
        /// Adds a console logger named 'Console' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        public static ILoggingBuilder AddRingConsole(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<RingLoggerProcessor>();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, RingConsoleLoggerProvider>());
            return builder;
        }
    }
}