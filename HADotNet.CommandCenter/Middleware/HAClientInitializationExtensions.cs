using Microsoft.AspNetCore.Builder;

namespace HADotNet.CommandCenter.Middleware
{
    public static class HAClientInitializationExtensions
    {
        public static IApplicationBuilder UseHAClientInitialization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HAClientInitialization>();
        }
    }
}
