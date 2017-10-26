using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;
using InteractiveHost;
using InteractiveHost.Logging;
using InteractiveHost.Commands;
using Microsoft.Extensions.Logging.Console.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.AspNetCore
{
    public static class ApiHost
    {
        public static IWebHostBuilder CreateDefaultBuilder(string[] args)
        {
            var interactive = false;
            foreach(var arg in args)
            {
                if(arg == "-i")
                {
                    interactive = true;
                    break;
                }
            }
            var builder = CreateBuilder(args, interactive);
            return builder;
        }

        private static IWebHostBuilder CreateBuilder(string[] args, bool interactive)
        {
            var builder = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    config.AddEnvironmentVariables();

                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    if(interactive)
                    {
                        logging.AddRingConsole();
                    }
                    else
                    {
                        logging.AddConsole();
                        logging.AddDebug();
                    }
                })
                .UseIISIntegration()
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .ConfigureServices(services => services.AddMvc(opt => 
                    {
                        opt.Conventions.Add(new ApiExplorerVisibilityEnabledConvention());     
                    }));

                if(interactive)
                {
                    builder.ConfigureServices(services => {
                        services.AddSingleton<InteractiveHostCommandFactory>();
                        services.AddSingleton<IConsole, AnsiLogConsole>(); //TODO: Copy code to make platform specific console.
                        services.AddSingleton<IAnsiSystemConsole, AnsiSystemConsole>();
                        services.TryAddEnumerable(ServiceDescriptor.Singleton<InteractiveHostCommand, ExitCommand>());
                        services.TryAddEnumerable(ServiceDescriptor.Singleton<InteractiveHostCommand, ConfigCommand>());
                        services.TryAddEnumerable(ServiceDescriptor.Singleton<InteractiveHostCommand, RoutesCommand>());
                        services.TryAddEnumerable(ServiceDescriptor.Singleton<InteractiveHostCommand, GetCommand>());
                        services.TryAddEnumerable(ServiceDescriptor.Singleton<InteractiveHostCommand, LogsCommand>());
                    });
                }

            return builder;
        }

        private class AnsiSystemConsole : IAnsiSystemConsole
        {
            public void Write(string message)
            {
                System.Console.Write(message);
            }

            public void WriteLine(string message)
            {
                System.Console.WriteLine(message);
            }
        }
    }
}
