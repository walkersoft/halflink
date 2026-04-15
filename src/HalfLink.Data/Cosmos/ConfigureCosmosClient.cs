using Azure.Identity;
using HalfLink.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HalfLink.Data.Cosmos
{
    internal static class ConfigureCosmosClient
    {
        internal static IServiceCollection ConfigureCosmos(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CosmosSettings>(configuration.GetSection(nameof(CosmosSettings)));
            services.AddSingleton(provider => provider.GetRequiredService<IOptions<CosmosSettings>>().Value);
            services.AddSingleton(provider =>
            {
                var cosmosSettings = provider.GetRequiredService<CosmosSettings>();

                ArgumentException.ThrowIfNullOrWhiteSpace(cosmosSettings.DatabaseName, nameof(cosmosSettings.DatabaseName));
                ArgumentException.ThrowIfNullOrWhiteSpace(cosmosSettings.LinksContainer, nameof(cosmosSettings.LinksContainer));
                ArgumentException.ThrowIfNullOrWhiteSpace(cosmosSettings.StatsContainer, nameof(cosmosSettings.StatsContainer));
                var cosmosOptions = new CosmosClientOptions
                {
                    ApplicationName = "HalfLink",
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                    }
                };

                if (cosmosSettings.UseManagedIdentity)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(cosmosSettings.Endpoint, nameof(cosmosSettings.Endpoint));
                    cosmosOptions.ConnectionMode = ConnectionMode.Gateway;
                    cosmosOptions.LimitToEndpoint = true;
                    var credential = string.IsNullOrWhiteSpace(cosmosSettings.ManagedIdentityClientId)
                        ? new DefaultAzureCredential()
                        : new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = cosmosSettings.ManagedIdentityClientId });
                    return new CosmosClient(cosmosSettings.Endpoint, credential, cosmosOptions);
                }

                ArgumentException.ThrowIfNullOrWhiteSpace(cosmosSettings.Endpoint, nameof(cosmosSettings.Endpoint));
                ArgumentException.ThrowIfNullOrWhiteSpace(cosmosSettings.Key, nameof(cosmosSettings.Key));
                return new CosmosClient(cosmosSettings.Endpoint, cosmosSettings.Key, cosmosOptions);
            });

            services.AddHostedService<CosmosInitializationService>();
            services.AddSingleton<IHalfLinkRepository, CosmosHalfLinkRepository>();
            services.AddSingleton<IHalfLinkStatRepository, CosmosHalfLinkStatRepository>();

            return services;
        }
    }
}
