using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HalfLink.Data
{
    public static class ConfigureAzureServices
    {
        public static IServiceCollection ConfigureAzure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AzureSettings>(configuration.GetSection(nameof(AzureSettings)));
            services.ConfigureAzureQueue(configuration);
            return services;
        }

        private static void ConfigureAzureQueue(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(provider => provider.GetRequiredService<IOptions<AzureSettings>>().Value);
            services.AddSingleton(provider =>
            {
                var queueSettings = provider.GetRequiredService<AzureSettings>().QueueSettings;
                ArgumentNullException.ThrowIfNull(queueSettings, nameof(queueSettings));
                ArgumentException.ThrowIfNullOrEmpty(queueSettings.ConnectionString, nameof(queueSettings.ConnectionString));
                ArgumentException.ThrowIfNullOrEmpty(queueSettings.QueueName, nameof(queueSettings.QueueName));

                var queueService = new QueueServiceClient(queueSettings.ConnectionString);
                var queueClient = queueService.GetQueueClient(queueSettings.QueueName);
                queueClient.CreateIfNotExists();

                return queueService;
            });
            services.AddSingleton<HalfLinkActivityQueue>();

        }
    }
}
