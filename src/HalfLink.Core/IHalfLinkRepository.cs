using HalfLink.Core.Entities;

namespace HalfLink.Core;

public interface IHalfLinkRepository
{
    Task CreateLink(Link link);
    Task<Link?> GetLink(string halfLink);
    Task<bool> HalfLinkExists(string halfLink);
}
