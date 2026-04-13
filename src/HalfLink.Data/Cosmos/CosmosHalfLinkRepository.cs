using HalfLink.Core;
using HalfLink.Core.Entities;
using Microsoft.Azure.Cosmos;

namespace HalfLink.Data.Cosmos
{
    internal sealed class CosmosHalfLinkRepository(CosmosClient client, CosmosSettings settings) : IHalfLinkRepository
    {
        public async Task CreateLink(Link link)
        {
            var container = GetLinksContainer();
            await container.CreateItemAsync(link, new PartitionKey(link.HalfLink));
        }

        public async Task<Link?> GetLink(string halfLink)
        {
            var container = GetLinksContainer();
            var query = new QueryDefinition("SELECT * FROM links WHERE links.halfLink = @halfLink")
                .WithParameter("@halfLink", halfLink);

            using var iterator = container.GetItemQueryIterator<Link>(query);
            if (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                return response.Resource.FirstOrDefault();
            }

            return null;
        }

        public async Task<bool> HalfLinkExists(string halfLink)
        {
            var container = GetLinksContainer();
            var query = new QueryDefinition("SELECT VALUE COUNT(1) FROM links WHERE links.halfLink = @halfLink")
                .WithParameter("@halfLink", halfLink);

            using var iterator = container.GetItemQueryIterator<int>(query);
            if (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                return response.Resource.FirstOrDefault() > 0;
            }

            return false;
        }

        private Container GetLinksContainer() => client.GetContainer(settings.DatabaseName, settings.LinksContainer);
    }
}
