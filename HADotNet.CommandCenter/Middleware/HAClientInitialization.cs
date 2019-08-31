using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Middleware
{
    public class HAClientInitialization
    {
        private RequestDelegate Next { get; }
        public IConfigStore ConfigStore { get; }
        public ILogger Log { get; }

        public HAClientInitialization(RequestDelegate next, IConfigStore configStore, ILogger<HAClientInitialization> log)
        {
            Next = next;
            Log = log;
            ConfigStore = configStore;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var config = await ConfigStore.GetConfigAsync();

            if (!string.IsNullOrWhiteSpace(config?.Settings?.BaseUri) && !string.IsNullOrWhiteSpace(config?.Settings?.AccessToken))
            {
                Log.LogInformation($"Initializing HA Client Factory with URL {config?.Settings?.BaseUri ?? "[NULL]"} and access token [{new string(config?.Settings?.AccessToken.Take(6).ToArray())}•••••••••••{new string(config?.Settings?.AccessToken.TakeLast(6).ToArray())}].");

                ClientFactory.Initialize(config.Settings.BaseUri, config.Settings.AccessToken);
            }

            if (!ClientFactory.IsInitialized)
            {
                if (context.Request.Path.ToString().ToLower() != "/admin/settings")
                {
                    Log.LogInformation($"Client factory is not initialized, redirecting user to settings area...");

                    context.Response.Redirect("/admin/settings?setup=1");
                    return;
                }
            }

            await Next(context);
        }
    }
}
