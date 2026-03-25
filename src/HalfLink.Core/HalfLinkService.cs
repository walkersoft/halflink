using HalfLink.Core.Entities;

namespace HalfLink.Core
{
    public sealed class HalfLinkService(IHalfLinkRepository repository)
    {
        const string halfLinkChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public async Task<Link> CreateLink(string url)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(url, nameof(url));
            url = url.ToLower();

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = $"http://{url}";
            }

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new ArgumentException("Invalid URL format", nameof(url));
            }

            var link = new Link
            {
                Id = Guid.NewGuid(),
                FullLink = url,
                HalfLink = GenerateHalfLink()
            };

            return link;
        }

        private static string GenerateHalfLink()
        {
            var random = new Random();

            var chars = new char[8];
            for (var i = 0; i < chars.Length; i++)
            {
                chars[i] = halfLinkChars[random.Next(halfLinkChars.Length)];
            }

            return new string([.. chars]);
        }
    }
}
