

using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging.Console.Internal;

namespace InteractiveHost.Commands
{
    public class ExitCommand : InteractiveHostCommand
    {
        private IApplicationLifetime _appLifetime;

        public ExitCommand(IApplicationLifetime appLifetime)
        {
            if (appLifetime == null)
            {
                throw new System.ArgumentNullException(nameof(appLifetime));
            }

            _appLifetime = appLifetime;
        }
        public override bool CanHandleCommand(string commandText)
        {
            return string.Equals("exit", commandText, System.StringComparison.CurrentCultureIgnoreCase);
        }

        public override Task ExecuteAsync(string commandText)
        {
            _appLifetime.StopApplication();
            return Task.CompletedTask;
        }
    }
}