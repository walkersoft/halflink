using HalfLink.Core.Entities;

namespace HalfLink.Core.Tests
{
    public class StubHalfLinkRepository : IHalfLinkRepository
    {
        private readonly Dictionary<string, Link> links = [];

        public Task CreateLink(Link link)
        {
            links[link.HalfLink] = link;
            return Task.CompletedTask;
        }

        public Task<Link?> GetLink(string halfLink)
        {
            links.TryGetValue(halfLink, out var link);
            return Task.FromResult(link);
        }

        public Task<bool> HalfLinkExists(string halfLink)
        {
            return Task.FromResult(links.ContainsKey(halfLink));
        }
    }
}
