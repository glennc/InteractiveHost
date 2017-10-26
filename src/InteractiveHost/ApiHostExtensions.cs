using System;
using System.Threading;
using System.Threading.Tasks;
using InteractiveHost;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace Microsoft.AspNetCore.Hosting
{
    public static class WebHostExtensions
    {
        public static void RunInteractive(this IWebHost host)
        {
            host.RunInteractiveAsync().GetAwaiter().GetResult();
        }

        public static async Task RunInteractiveAsync(this IWebHost host, CancellationToken token = default(CancellationToken))
        {
            // If token cannot be canceled, attach Ctrl+C and SIGTERM shutdown
            var done = new ManualResetEventSlim(false);
            using (var cts = new CancellationTokenSource())
            {
                AttachCtrlcSigtermShutdown(cts, done, shutdownMessage: "Application is shutting down...");

                await host.RunInteractiveAsync(cts.Token, "Interactive application explorer started. Type exit<enter> or Ctrl + C to shut down.");
                done.Set();
            }
        }

        public static async Task RunInteractiveAsync(this IWebHost host, CancellationToken token, string message)
        {
            using (host)
            {
                await host.StartAsync(token);

                var hostingEnvironment = host.Services.GetService<IHostingEnvironment>();
                var applicationLifetime = host.Services.GetService<IApplicationLifetime>();

                Console.WriteLine($"Hosting environment: {hostingEnvironment.EnvironmentName}");
                Console.WriteLine($"Content root path: {hostingEnvironment.ContentRootPath}");

                var serverAddresses = host.ServerFeatures.Get<IServerAddressesFeature>()?.Addresses;
                if (serverAddresses != null)
                {
                    foreach (var address in serverAddresses)
                    {
                        Console.WriteLine($"Now listening on: {address}");
                    }
                }

                if (!string.IsNullOrEmpty(message))
                {
                    Console.WriteLine(message);
                }

                var commandFactory = host.Services.GetRequiredService<InteractiveHostCommandFactory>();
                while(!token.IsCancellationRequested &&
                      !applicationLifetime.ApplicationStopping.IsCancellationRequested)
                {
                    Console.Write("Command > ");

                    var commandText = Console.ReadLine();

                    if (commandText == null)
                    {
                        //null command text happens when you ctrl + c the app.
                        break;
                    }

                    var command = commandFactory.Get(commandText);

                    if(command == null)
                    {
                        Console.WriteLine("No command registered that can handle " + commandText);
                    }
                    else
                    {
                        await command.ExecuteAsync(commandText);
                    }
                }
            }
        }

        private static void AttachCtrlcSigtermShutdown(CancellationTokenSource cts, ManualResetEventSlim resetEvent, string shutdownMessage)
        {
            void Shutdown()
            {
                if (!cts.IsCancellationRequested)
                {
                    if (!string.IsNullOrEmpty(shutdownMessage))
                    {
                        Console.WriteLine(shutdownMessage);
                    }
                    try
                    {
                        cts.Cancel();
                    }
                    catch (ObjectDisposedException) { }
                }

                // Wait on the given reset event
                resetEvent.Wait();
            };

            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => Shutdown();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Shutdown();
                // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
                eventArgs.Cancel = true;
            };
        }
    }
}