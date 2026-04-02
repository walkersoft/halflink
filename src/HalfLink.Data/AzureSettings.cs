namespace HalfLink.Data
{
    internal class AzureSettings
    {
        public QueueSettings Queue { get; set; } = new QueueSettings();
    }

    internal class QueueSettings
    {
        public string QueueName { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
    }
}
