using HalfLink.Core.Entities;

namespace HalfLink.Core
{
    public sealed class HalfLinkService(IHalfLinkRepository repository)
    {
        const string halfLinkChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public async Task<Link> CreateHalfLink(string url)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(url, nameof(url));
            url = url.ToLower();

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = $"http://{url}";
            }

            if (!IsValidUrl(url)) throw new ArgumentException("Invalid URL format", nameof(url));

            var halfLink = new Link
            {
                Id = Guid.NewGuid(),
                FullLink = url,
                HalfLink = GenerateHalfLink()
            };

            return halfLink;
        }

        private static bool IsValidUrl(string url) =>
            Uri.TryCreate(url, UriKind.Absolute, out var _) && Uri.IsWellFormedUriString(url, UriKind.Absolute);

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
