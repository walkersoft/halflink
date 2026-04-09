using Azure.Identity;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HalfLink.Data
{
    internal static class ConfigureAzureServices
    {
        internal static IServiceCollection ConfigureAzure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AzureSettings>(configuration.GetSection(nameof(AzureSettings)));
            services.AddSingleton(provider => provider.GetRequiredService<IOptions<AzureSettings>>().Value);
            services.ConfigureAzureQueue();
            return services;
        }

        private static void ConfigureAzureQueue(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var azureSettings = provider.GetRequiredService<AzureSettings>();
                var queueSettings = azureSettings.QueueSettings;
                ArgumentException.ThrowIfNullOrEmpty(queueSettings.QueueName, nameof(queueSettings.QueueName));

                QueueServiceClient queueService;
                if (azureSettings.UseManagedIdentity)
                {
                    ArgumentException.ThrowIfNullOrEmpty(queueSettings.ServiceUri, nameof(queueSettings.ServiceUri));
                    queueService = new QueueServiceClient(new Uri(queueSettings.ServiceUri), new DefaultAzureCredential());
                }
                else
                {
                    ArgumentException.ThrowIfNullOrEmpty(queueSettings.ConnectionString, nameof(queueSettings.ConnectionString));
                    queueService = new QueueServiceClient(queueSettings.ConnectionString);
                }

                var queueClient = queueService.GetQueueClient(queueSettings.QueueName);
                queueClient.CreateIfNotExists();

                return queueService;
            });
            services.AddSingleton<HalfLinkActivityQueue>();
        }
    }
}
