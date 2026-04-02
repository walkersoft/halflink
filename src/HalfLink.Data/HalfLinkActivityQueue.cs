using Azure.Storage.Queues;
using System.Text.Json;

namespace HalfLink.Data
{
    public record ClickActivityEvent
    {
        public Guid LinkId { get; set; }
        public string Referrer { get; set; } = string.Empty;
        public DateTime ClickedAt { get; set; }
    }

    public class HalfLinkActivityQueue(AzureSettings azureSettings, QueueServiceClient queueServiceClient)
    {
        private readonly QueueClient queueClient = queueServiceClient.GetQueueClient(azureSettings.QueueSettings.QueueName);
        private readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);

        public Task AddClickActivity(ClickActivityEvent clickActivityEvent)
        {
            return queueClient.SendMessageAsync(JsonSerializer.Serialize(clickActivityEvent, options: serializerOptions));
        }
    }
}
