﻿using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.Core;
using Microsoft.AspNetCore.Http;
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

        public HAClientInitialization(RequestDelegate next, IConfigStore configStore)
        {
            Next = next;
            ConfigStore = configStore;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var config = await ConfigStore.GetConfigAsync();

            if (!string.IsNullOrWhiteSpace(config?.Settings?.BaseUri) && !string.IsNullOrWhiteSpace(config?.Settings?.AccessToken))
            {
                ClientFactory.Initialize(config.Settings.BaseUri, config.Settings.AccessToken);
            }

            if (!ClientFactory.IsInitialized)
            {
                if (context.Request.Path.ToString().ToLower() != "/admin/settings")
                {
                    context.Response.Redirect("/admin/settings?setup=1");
                    return;
                }
            }

            await Next(context);
        }
    }
}