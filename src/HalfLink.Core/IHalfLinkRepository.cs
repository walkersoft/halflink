using HalfLink.Core.Entities;

namespace HalfLink.Core;

public interface IHalfLinkRepository
{
    Task CreateLink(Link link);
    Task<bool> HalfLinkExists(string halfLink);
}
