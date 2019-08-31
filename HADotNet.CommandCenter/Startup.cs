using HADotNet.CommandCenter.Hubs;  
using HADotNet.CommandCenter.Middleware;
using HADotNet.CommandCenter.Models;
using HADotNet.CommandCenter.Services;
using HADotNet.CommandCenter.Services.Interfaces;
using HADotNet.Core;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HADotNet.CommandCenter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services, IHostingEnvironment env)
        {
            services.AddOptions();
            services.AddLogging(l =>
            {
                l.ClearProviders();

                l.AddConsole();
                if (env.IsDevelopment())
                {
                    l.AddDebug();
                    l.SetMinimumLevel(LogLevel.Debug);
                    l.AddFilter("Microsoft", LogLevel.Information);
                    l.AddFilter("System", LogLevel.Information);
                }
                else
                {
                    l.SetMinimumLevel(LogLevel.Information);
                    l.AddFilter("Microsoft", LogLevel.Error);
                    l.AddFilter("System", LogLevel.Warning);
                }
            });

            services.Configure<HaccOptions>(Configuration.GetSection("HACC"));

            services.AddSingleton<IConfigStore, JsonConfigStore>();
            services.AddScoped(_ => ClientFactory.GetClient<EntityClient>());
            services.AddScoped(_ => ClientFactory.GetClient<StatesClient>());
            services.AddScoped(_ => ClientFactory.GetClient<ServiceClient>());

            services.AddSignalR();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseHAClientInitialization();

            app.UseSignalR(routes =>
            {
                routes.MapHub<TileHub>("/hubs/tile");
            });

            app.UseMvcWithDefaultRoute();
        }
    }
}
