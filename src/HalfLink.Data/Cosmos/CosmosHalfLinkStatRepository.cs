using HalfLink.Core;
using HalfLink.Core.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace HalfLink.Data.Cosmos
{
    internal class CosmosHalfLinkStatRepository(CosmosClient client, CosmosSettings settings) : IHalfLinkStatRepository
    {
        public async Task CreateStat(LinkStat linkStat)
        {
            var container = GetStatsContainer();
            await container.CreateItemAsync(linkStat, new PartitionKey(linkStat.LinkId.ToString()));
        }

        public async Task<IEnumerable<LinkStat>> GetStats(Guid halfLinkId)
        {
            var container = GetStatsContainer();
            var queryable = container.GetItemLinqQueryable<LinkStat>();

            var stats = queryable
                .Where(stat => stat.LinkId == halfLinkId)
                .ToFeedIterator();

            List<LinkStat> results = [];
            while (stats.HasMoreResults)
            {
                var response = await stats.ReadNextAsync();
                results.AddRange(response.Resource);
            }

            return results;
        }

        private Container GetStatsContainer() => client.GetContainer(settings.DatabaseName, settings.StatsContainer);
    }
}
