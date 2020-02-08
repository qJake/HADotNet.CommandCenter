using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace HADotNet.CommandCenter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(wb =>
                {
                    wb.ConfigureKestrel(k => k.AddServerHeader = false);
                    wb.CaptureStartupErrors(true);
                    wb.UseStartup<Startup>();
                });
    }
}
