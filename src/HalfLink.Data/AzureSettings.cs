namespace HalfLink.Data
{
    public class AzureSettings
    {
        public QueueSettings QueueSettings { get; set; } = new QueueSettings();
    }

    public class QueueSettings
    {
        public string QueueName { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
    }
}
