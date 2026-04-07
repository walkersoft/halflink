using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HalfLink.Data
{
    public static class ConfigureServices
    {
        public static IServiceCollection ConfigureDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<LocalLinkStore>();
            return services;
        }
    }
}
