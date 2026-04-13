using Azure.Storage.Queues;
using HalfLink.Core;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HalfLink.Data.Queue
{
    internal class HalfLinkActivityQueue(AzureSettings azureSettings, QueueServiceClient queueServiceClient, ILogger<HalfLinkActivityQueue> logger) : IHalfLinkActivityService
    {
        private readonly QueueClient queueClient = queueServiceClient.GetQueueClient(azureSettings.QueueSettings.QueueName);

        public async Task AddClickActivity(ClickActivityEvent clickActivityEvent)
        {
            logger.LogTrace("Sending click event to queue (linkId: {LinkId})", clickActivityEvent.LinkId);
            try
            {
                var json = JsonSerializer.Serialize(clickActivityEvent, options: new(JsonSerializerDefaults.Web));
                var response = await queueClient.SendMessageAsync(json);
                logger.LogInformation(
                    "Click event queued (linkId: {LinkId}, messageId: {MessageId})",
                    clickActivityEvent.LinkId,
                    response.Value.MessageId
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to queue click event (linkId: {LinkId})", clickActivityEvent.LinkId);
                throw;
            }
        }
    }
}
