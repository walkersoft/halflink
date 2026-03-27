using HalfLink.Core.Entities;

namespace HalfLink.Core
{
    public interface IHalfLinkStatRepository
    {
        void CreateStatEntry(LinkStat linkStat);
    }
}
