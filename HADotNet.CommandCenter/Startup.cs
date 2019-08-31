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

namespace HADotNet.CommandCenter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

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
