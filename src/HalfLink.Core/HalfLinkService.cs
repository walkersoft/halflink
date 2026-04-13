using HalfLink.Core.Entities;

namespace HalfLink.Core
{
    public sealed class HalfLinkService(IHalfLinkRepository linksRepository, IHalfLinkStatRepository statsRepository, IHalfLinkActivityService halfLinkActivityService)
    {
        const string HALF_LINK_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        const int MAX_GENERATIONS = 3;

        public async Task<Link?> GetLink(string halfLink) => await linksRepository.GetLink(halfLink);

        public async Task<Link> CreateLink(string url, string? description = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(url, nameof(url));
            url = url.Trim();

            if (!url.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) && !url.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                url = $"https://{url}";
            }

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new ArgumentException("Invalid URL format", nameof(url));
            }

            var halfLink = await GenerateHalfLink();

            var link = new Link
            {
                Id = Guid.NewGuid(),
                FullLink = url,
                HalfLink = halfLink,
                Description = description,
            };

            await linksRepository.CreateLink(link);

            var firstStat = new LinkStat
            {
                Id = Guid.NewGuid(),
                LinkId = link.Id,
                AccessedAt = DateTime.UtcNow,
                Referrer = "SYSTEM_CREATED"
            };

            await statsRepository.CreateStat(firstStat);

            return link;
        }

        private async Task<string> GenerateHalfLink()
        {
            var generations = MAX_GENERATIONS;
            bool halfLinkExists;
            string halfLink;

            do
            {
                halfLink = CreateRandomHalfLink();
                halfLinkExists = await linksRepository.HalfLinkExists(halfLink);
                generations--;
            } while (halfLinkExists && generations > 0);

            if (halfLinkExists)
            {
                throw new InvalidOperationException($"Failed to generate a unique half link after {MAX_GENERATIONS} attempts");
            }

            return halfLink;
        }

        private static string CreateRandomHalfLink()
        {
            var random = new Random();

            var chars = new char[8];
            for (var i = 0; i < chars.Length; i++)
            {
                chars[i] = HALF_LINK_CHARS[random.Next(HALF_LINK_CHARS.Length)];
            }

            return new string([.. chars]);
        }

        public async Task<IEnumerable<LinkStat>> GetStats(string halfLink)
        {
            var link = await linksRepository.GetLink(halfLink);
            if (link is null) return [];

            return await statsRepository.GetStats(link.Id);
        }

        public Task CreateStat(LinkStat stat) => statsRepository.CreateStat(stat);

        public Task AddClickActivity(ClickActivityEvent clickActivityEvent) => halfLinkActivityService.AddClickActivity(clickActivityEvent);
    }
}
