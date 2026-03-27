using HalfLink.Core;
using HalfLink.Core.Entities;
using Microsoft.Azure.Cosmos;

namespace HalfLink.Data
{
    internal class CosmosHalfLinkStatRepository(CosmosClient client, CosmosSettings settings) : IHalfLinkStatRepository
    {
        public async Task CreateStat(LinkStat linkStat)
        {
            var container = GetStatsContainer();
            await container.CreateItemAsync(linkStat, new PartitionKey(linkStat.LinkId.ToString()));
        }

        private Container GetStatsContainer() => client.GetContainer(settings.DatabaseName, settings.StatsContainer);
    }
}
