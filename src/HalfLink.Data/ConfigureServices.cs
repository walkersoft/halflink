using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HalfLink.Data
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureCosmos(configuration);
            services.ConfigureAzure(configuration);
            services.AddScoped<LocalLinkStore>();

            return services;
        }
    }
}
