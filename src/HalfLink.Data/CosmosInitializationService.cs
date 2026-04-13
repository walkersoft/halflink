using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HalfLink.Data
{
    internal sealed class CosmosInitializationService(CosmosClient client, CosmosSettings settings, ILogger<CosmosInitializationService> logger) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Initializing Cosmos DB...");
                var db = await client.CreateDatabaseIfNotExistsAsync(settings.DatabaseName, cancellationToken: cancellationToken);
                await db.Database.CreateContainerIfNotExistsAsync(new(settings.LinksContainer, "/halfLink"), cancellationToken: cancellationToken);
                await db.Database.CreateContainerIfNotExistsAsync(new(settings.StatsContainer, "/linkId"), cancellationToken: cancellationToken);
                logger.LogInformation("Cosmos DB initialized successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to initialize Cosmos DB");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
