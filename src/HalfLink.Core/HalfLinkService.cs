using HalfLink.Core.Entities;

namespace HalfLink.Core
{
    public sealed class HalfLinkService(IHalfLinkRepository repository)
    {
        const string HALF_LINK_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        const int MAX_GENERATIONS = 3;

        public async Task<Link> CreateLink(string url)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(url, nameof(url));
            url = url.ToLower();

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
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
                HalfLink = halfLink
            };

            await repository.CreateLink(link);

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
                halfLinkExists = await repository.HalfLinkExists(halfLink);
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

        public async Task<Link?> GetLink(string halfLink)
        {
            throw new NotImplementedException();
        }
    }
}
