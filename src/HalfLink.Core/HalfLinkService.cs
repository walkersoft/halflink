using HalfLink.Core.Entities;

namespace HalfLink.Core
{
    public sealed class HalfLinkService(IHalfLinkRepository repository)
    {
        public async Task<Link> CreateHalfLink(string url)
        {
            throw new NotImplementedException();
        }
    }
}
