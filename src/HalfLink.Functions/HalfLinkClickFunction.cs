using Microsoft.Azure.Functions.Worker;
using System.Net.Http.Json;

namespace HalfLink.Functions
{
    internal class HalfLinkClickFunction(IHttpClientFactory httpClientFactory)
    {
        internal record ClickActivityEvent
        {
            public Guid LinkId { get; set; }
            public string Referrer { get; set; } = string.Empty;
            public DateTime ClickedAt { get; set; }
        }

        [Function(nameof(ReadHalfLinkClicks))]
        public async Task ReadHalfLinkClicks([QueueTrigger("halflink-clicks", Connection = "AzureQueueStorage")] ClickActivityEvent message)
        {
            var httpClient = httpClientFactory.CreateClient("HalfLinkApiClient");
            var response = await httpClient.PostAsJsonAsync("/s/click", message);
            response.EnsureSuccessStatusCode();
        }
    }
}
