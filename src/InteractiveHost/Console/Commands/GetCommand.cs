

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Logging.Console.Internal;

namespace InteractiveHost.Commands
{
    public class GetCommand : InteractiveHostCommand
    {
        private IServer _server;
        private HttpClient _client;

        public GetCommand(IServer host)
        {
            _server = host ?? throw new ArgumentNullException(nameof(host));
            _client = new HttpClient();
        }

        public override bool CanHandleCommand(string commandText)
        {
            return commandText.StartsWith("get", StringComparison.CurrentCultureIgnoreCase);
        }

        public override async Task ExecuteAsync(string commandText)
        {
            var serverAddresses = _server.Features.Get<IServerAddressesFeature>()?.Addresses;
            var uri = new Uri(new Uri(serverAddresses.First()), commandText.Replace("GET ", ""));
            Console.WriteLine("Getting: " + uri.AbsoluteUri);
            try
            {
                var result = await _client.GetStringAsync(uri);
                Console.WriteLine(result);
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine($"Unable to get {uri.AbsoluteUri}: {e.Message}");
            }
        }
    }
}