using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace HalfLink.Functions
{
    internal class HalfLinkClickFunction(IHttpClientFactory httpClientFactory, ILogger<HalfLinkClickFunction> logger)
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
            logger.LogInformation("Queue trigger fired (linkId: {LinkId}, clickedAt: {ClickedAt})", message.LinkId, message.ClickedAt);

            try
            {
                var httpClient = httpClientFactory.CreateClient("HalfLinkApiClient");
                logger.LogTrace("Posting click to {BaseAddress}/s/click (linkId: {LinkId})", httpClient.BaseAddress, message.LinkId);

                var response = await httpClient.PostAsJsonAsync("/s/click", message);

                logger.LogTrace("POST /s/click response: {StatusCode} (linkId: {LinkId})", response.StatusCode, message.LinkId);

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process click event (linkId: {LinkId})", message.LinkId);
                throw;
            }
        }
    }
}
