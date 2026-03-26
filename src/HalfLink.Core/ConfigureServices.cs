using Microsoft.Extensions.DependencyInjection;

namespace HalfLink.Core
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<HalfLinkService>();
            return services;
        }
    }
}
