

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Console.Internal;

namespace InteractiveHost.Commands
{
    public class ConfigCommand : InteractiveHostCommand
    {
        private IConfiguration _config;

        public ConfigCommand(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public override bool CanHandleCommand(string commandText)
        {
            return commandText.Equals("config", StringComparison.CurrentCultureIgnoreCase);
        }

        public override Task ExecuteAsync(string commandText)
        {
            Console.WriteLine("Writing configuration keys:");
            foreach(var key in _config.AsEnumerable())
            {
                Console.WriteLine($"K: {key.Key, -40} V: {key.Value}");
            }
            return Task.CompletedTask;
        }
    }
}