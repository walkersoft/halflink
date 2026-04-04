using Azure.Storage.Queues;
using System.Text.Json;

namespace HalfLink.Data
{
    public record ClickActivityEvent(Guid LinkId, string Referrer, DateTime ClickedAt);

    public class HalfLinkActivityQueue(AzureSettings azureSettings, QueueServiceClient queueServiceClient)
    {
        private readonly QueueClient queueClient = queueServiceClient.GetQueueClient(azureSettings.QueueSettings.QueueName);

        public Task AddClickActivity(ClickActivityEvent clickActivityEvent)
        {
            return queueClient.SendMessageAsync(JsonSerializer.Serialize(clickActivityEvent, options: new(JsonSerializerDefaults.Web)));
        }
    }
}
