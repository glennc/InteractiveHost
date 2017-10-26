using System;
using System.Collections.Generic;

namespace InteractiveHost
{
    public class InteractiveHostCommandFactory
    {
        IEnumerable<InteractiveHostCommand> _commands;

        public InteractiveHostCommandFactory(IEnumerable<InteractiveHostCommand> commands)
        {
            _commands = commands;
        }

        public InteractiveHostCommand Get(string commandText)
        {
            foreach(var command in _commands)
            {
                if(command.CanHandleCommand(commandText))
                {
                    return command;
                }
            }
            return null;
        }
    }
}