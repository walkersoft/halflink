using HalfLink.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HalfLink.Data
{
    public static class ConfigureCosmosClient
    {
        public static IServiceCollection ConfigureCosmos(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CosmosSettings>(configuration.GetSection(nameof(CosmosSettings)));
            services.AddSingleton(provider => provider.GetRequiredService<IOptions<CosmosSettings>>().Value);
            services.AddSingleton(provider =>
            {
                var cosmosSettings = provider.GetRequiredService<CosmosSettings>();

                ArgumentException.ThrowIfNullOrWhiteSpace(cosmosSettings.Endpoint, nameof(cosmosSettings.Endpoint));
                ArgumentException.ThrowIfNullOrWhiteSpace(cosmosSettings.Key, nameof(cosmosSettings.Key));
                ArgumentException.ThrowIfNullOrWhiteSpace(cosmosSettings.DatabaseName, nameof(cosmosSettings.DatabaseName));
                ArgumentException.ThrowIfNullOrWhiteSpace(cosmosSettings.LinksContainer, nameof(cosmosSettings.LinksContainer));
                ArgumentException.ThrowIfNullOrWhiteSpace(cosmosSettings.StatsContainer, nameof(cosmosSettings.StatsContainer));
                var cosmosOptions = new CosmosClientOptions
                {
                    ApplicationName = "HalfLink",
                    ConnectionMode = ConnectionMode.Gateway,
                    LimitToEndpoint = true,
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                    }
                };

                return new CosmosClient(cosmosSettings.Endpoint, cosmosSettings.Key, cosmosOptions);
            });

            services.AddHostedService<CosmosInitializationService>();
            services.AddSingleton<IHalfLinkRepository, CosmosHalfLinkRepository>();

            return services;
        }
    }
}
