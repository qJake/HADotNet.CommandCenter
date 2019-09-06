using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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
                if (!ClientFactory.IsInitialized)
                {
                    if (config.Settings.IsHassIo)
                    {
                        Log.LogInformation($"Auto-initializing HACC via Hass.io addon.");
                    }
                    else
                    {
                        Log.LogInformation($"Initializing HACC API with URL {config?.Settings?.BaseUri ?? "[NULL]"} and access token [{new string(config?.Settings?.AccessToken.Take(6).ToArray())}•••••••••••{new string(config?.Settings?.AccessToken.TakeLast(6).ToArray())}].");
                    }
                }

                ClientFactory.Initialize(config.Settings.BaseUri, config.Settings.AccessToken);
            }
            else
            {
                ClientFactory.Reset();
            }

            if (!ClientFactory.IsInitialized)
            {
                var hassToken = Environment.GetEnvironmentVariable("HASSIO_TOKEN");

                if (!string.IsNullOrWhiteSpace(hassToken))
                {
                    await ConfigStore.ManipulateConfig(c =>
                    {
                        c.Settings.BaseUri = "http://hassio/homeassistant";
                        c.Settings.AccessToken = hassToken;
                        c.Settings.IsHassIo = true;

                        context.Response.StatusCode = 303;
                        context.Response.Redirect("/admin");
                    });
                }

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
