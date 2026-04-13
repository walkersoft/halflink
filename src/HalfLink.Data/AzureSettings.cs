namespace HalfLink.Data
{
    public class AzureSettings
    {
        public bool UseManagedIdentity { get; set; }
        public string ManagedIdentityClientId { get; set; } = string.Empty;
        public required QueueSettings QueueSettings { get; set; }
    }

    public class QueueSettings
    {
        public required string QueueName { get; set; }
        public string ConnectionString { get; set; } = string.Empty;
        public string ServiceUri { get; set; } = string.Empty;
    }
}
