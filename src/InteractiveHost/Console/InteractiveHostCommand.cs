
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Console.Internal;

namespace InteractiveHost
{
    public abstract class InteractiveHostCommand
    {
        public InteractiveHostCommand()
        {
        }

        public abstract bool CanHandleCommand(string commandText);

        public abstract Task ExecuteAsync(string commandText);
    }
}