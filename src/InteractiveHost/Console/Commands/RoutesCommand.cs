

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Logging.Console.Internal;

namespace InteractiveHost.Commands
{
    public class RoutesCommand : InteractiveHostCommand
    {
        private IApiDescriptionGroupCollectionProvider _apiExplorer;

        public RoutesCommand(IApiDescriptionGroupCollectionProvider apiExplorer)
        {
            _apiExplorer = apiExplorer ?? throw new ArgumentNullException(nameof(apiExplorer));
        }

        public override bool CanHandleCommand(string commandText)
        {
            return commandText.Equals("routes", StringComparison.CurrentCultureIgnoreCase);
        }

        public override Task ExecuteAsync(string commandText)
        {
            if(_apiExplorer.ApiDescriptionGroups.Items.Count == 0)
            {
                Console.WriteLine("No routes found in API Explorer.");
                return Task.CompletedTask;
            }

            foreach(var group in _apiExplorer.ApiDescriptionGroups.Items)
            {
                Console.WriteLine($"{group.GroupName ?? "Unknown group"}:");
                foreach(var api in group.Items)
                {
                    Console.WriteLine($"{api.HttpMethod, 10} {api.RelativePath}");
                }
            }
            return Task.CompletedTask;
        }
    }
}