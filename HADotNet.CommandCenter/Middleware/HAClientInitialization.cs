using HADotNet.CommandCenter.Models.Config;
using HADotNet.CommandCenter.Models.Config.Pages;
using HADotNet.CommandCenter.Models.Config.Tiles;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.CommandCenter.Utils;
using HADotNet.Core;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

            if (!ClientFactory.IsInitialized)
            {
                InitializeOrReset(config);

                if (!ClientFactory.IsInitialized)
                {
                    // If we're in Hass.io mode, set the base URI and redirect to the admin homepage.
                    if (SupervisorEnvironment.IsSupervisorAddon)
                    {
                        await AttemptBaseUrlDiscovery();

                        InitializeOrReset(config, true);
                    }

                    // If we aren't on one of the approved pages, redirect to the settings page and prompt for setup.
                    if (context.Request.Path.ToString().ToLower() != "/admin/settings" 
                        && context.Request.Path.ToString().ToLower() != "/admin/pagemigration"
                        && context.Request.Path.ToString().ToLower() != "/admin")
                    {
                        Log.LogInformation($"HA connection is not initialized, redirecting user to settings area...");

                        context.Response.Redirect("/admin/settings?setup=1");
                        return;
                    }
                }
            }

            // Pages Migration
            await MigrateLegacyPagesConfig(context, config);

            await Next(context);
        }

        private async Task AttemptBaseUrlDiscovery()
        {
            // Temporary while we fetch some stuff...
            ClientFactory.Initialize(SupervisorEnvironment.GetBaseUrl(), SupervisorEnvironment.GetSupervisorToken());

            var discovery = ClientFactory.GetClient<DiscoveryClient>();
            var discInfo = await discovery.GetDiscoveryInfo();

            if (string.IsNullOrWhiteSpace(discInfo?.BaseUrl))
            {
                Log.LogError("Unable to read discovery info from Home Assistant. Do you have the HTTP component configured in Home Assistant with a \"base_url\" set?");
                ClientFactory.Reset();
            }
            else
            {
                await ConfigStore.ManipulateConfig(c =>
                {
                    c.Settings = new SystemSettings
                    {
                        BaseUri = discInfo.BaseUrl
                    };
                });
            }
        }

        private void InitializeOrReset(ConfigRoot config, bool resetFirst = false)
        {
            if (resetFirst) ClientFactory.Reset();

            if (!string.IsNullOrWhiteSpace(config?.Settings?.BaseUri) && !string.IsNullOrWhiteSpace(config?.Settings?.AccessToken))
            {
                Log.LogInformation($"Initializing HACC API with URL {config?.Settings?.BaseUri ?? "[NULL]"} and access token [{new string(config?.Settings?.AccessToken.Take(6).ToArray())}•••••••••••{new string(config?.Settings?.AccessToken.TakeLast(6).ToArray())}].");
                ClientFactory.Initialize(config.Settings.BaseUri, config.Settings.AccessToken);
            }
            else
            {
                Log.LogWarning($"Can't initialize HACC API, missing the {(string.IsNullOrWhiteSpace(config?.Settings?.BaseUri) ? "base URL" : string.IsNullOrWhiteSpace(config?.Settings?.AccessToken) ? "access token" : "[unknown]")}.");
                ClientFactory.Reset();
            }
        }

        private async Task MigrateLegacyPagesConfig(HttpContext context, ConfigRoot config)
        {

#pragma warning disable CS0612

            if ((config.TileLayout?.Count > 0 || config.Tiles?.Count > 0) && (config.Pages?.Count ?? 0) == 0)
            {
                await ConfigStore.ManipulateConfig(config =>
                {
                    config.Pages ??= new List<Page>();
                    config.Pages.Add(new Page
                    {
                        Name = "default",
                        Description = "[Automatically generated from previous configuration.]",
                        IsDefaultPage = true,
                        Tiles = config.Tiles,
                        TileLayout = config.TileLayout,
                        LayoutSettings = config.LayoutSettings
                    });
                    config.TileLayout = null;
                    config.Tiles = null;
                    config.LayoutSettings = null;
                });
                context.Response.Redirect("/admin/pageMigration");
            }
            else if ((config.Pages?.Count ?? 0) == 0)
            {
                await ConfigStore.ManipulateConfig(config =>
                {
                    config.Pages = new List<Page>
                    {
                        new Page
                        {
                            Name = "default",
                            Description = "Default Page",
                            IsDefaultPage = true,
                            Tiles = new List<BaseTile>(),
                            TileLayout = new List<TileLayout>(),
                            LayoutSettings = new LayoutSettings
                            {
                                DeviceHeightPx = 1280,
                                DeviceWidthPx = 720,
                                BaseTileSizePx = 92,
                                TileSpacingPx = 6,
                            }
                        }
                    };
                });
            }

#pragma warning restore CS0612

        }
    }
}
