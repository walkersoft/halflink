using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Hosting;

namespace HalfLink.Data
{
    internal sealed class CosmosInitializationService(CosmosClient client, CosmosSettings settings) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var db = await client.CreateDatabaseIfNotExistsAsync(settings.DatabaseName, cancellationToken: cancellationToken);

            await db.Database.CreateContainerIfNotExistsAsync(
                new(settings.LinksContainer, "/halfLink"),
                cancellationToken: cancellationToken
            );

            await db.Database.CreateContainerIfNotExistsAsync(
                new(settings.StatsContainer, "/linkId"),
                cancellationToken: cancellationToken
            );
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
