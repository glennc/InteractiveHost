

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Logging.Console.Internal;
using InteractiveHost.Logging;

namespace InteractiveHost.Commands
{
    public class LogsCommand : InteractiveHostCommand
    {
        private RingLoggerProcessor _logProcessor;

        public LogsCommand(RingLoggerProcessor logProcessor)
        {
            _logProcessor = logProcessor ?? throw new ArgumentNullException(nameof(logProcessor));
        }

        public override bool CanHandleCommand(string commandText)
        {
            return commandText.Equals("logs", StringComparison.CurrentCultureIgnoreCase);
        }

        public override Task ExecuteAsync(string commandText)
        {
            _logProcessor.WriteAllLogs();
            return Task.CompletedTask;
        }
    }
}