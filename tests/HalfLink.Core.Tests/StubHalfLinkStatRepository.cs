using HalfLink.Core.Entities;

namespace HalfLink.Core.Tests
{
    public class StubHalfLinkStatRepository : IHalfLinkStatRepository
    {
        private readonly Dictionary<Guid, List<LinkStat>> stats = [];

        public Task CreateStat(LinkStat linkStat)
        {
            var list = stats.GetValueOrDefault(linkStat.LinkId, []);
            list.Add(linkStat);
            stats[linkStat.LinkId] = list;
            return Task.CompletedTask;
        }

        public Task<IEnumerable<LinkStat>> GetStats(Guid halfLinkId)
        {
            return Task.FromResult<IEnumerable<LinkStat>>(stats.GetValueOrDefault(halfLinkId)
                ?? throw new KeyNotFoundException($"No stats found for HalfLinkId: {halfLinkId}"));
        }
    }
}
