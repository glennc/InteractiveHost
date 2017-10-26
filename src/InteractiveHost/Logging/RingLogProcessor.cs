

using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging.Console.Internal;
namespace InteractiveHost.Logging
{
    public class RingLoggerProcessor : ConsoleLoggerProcessor
    {
        //TODO: Use options.
        private const int _maxQueuedMessages = 20;

        private readonly BlockingCollection<LogMessageEntry> _messageQueue = new BlockingCollection<LogMessageEntry>(_maxQueuedMessages);

        public override void EnqueueMessage(LogMessageEntry message)
        {
            try
            {
                if(_messageQueue.Count == _maxQueuedMessages)
                {
                    //Throw away a message so we can take its spot.
                    _messageQueue.Take();
                }
                _messageQueue.Add(message);
                return;
            }
            catch (InvalidOperationException) { }
        }

        public void WriteAllLogs()
        {
            if(_messageQueue.Count == 0)
            {
                Console.WriteLine("No log messages", null, null);
                return;
            }
            foreach (var message in _messageQueue)
            {
                if (message.LevelString != null)
                {
                    Console.Write(message.LevelString, message.LevelBackground, message.LevelForeground);
                }

                Console.Write(message.Message, message.MessageColor, message.MessageColor);
                Console.Flush();
            }
        }
    }
}