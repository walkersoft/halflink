using HalfLink.Core;
using HalfLink.Data.Cosmos;
using HalfLink.Data.Queue;
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
            services.AddSingleton<IHalfLinkActivityService, HalfLinkActivityQueue>();
            services.AddScoped<LocalLinkStore>();

            return services;
        }
    }
}
