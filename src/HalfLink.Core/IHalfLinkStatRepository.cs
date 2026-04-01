using HalfLink.Core.Entities;

namespace HalfLink.Core
{
    public interface IHalfLinkStatRepository
    {
        Task CreateStat(LinkStat linkStat);
        Task<IEnumerable<LinkStat>> GetStats(Guid halfLinkId);
    }
}
