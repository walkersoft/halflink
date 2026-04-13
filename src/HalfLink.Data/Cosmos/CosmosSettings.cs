namespace HalfLink.Data.Cosmos
{
    public class CosmosSettings
    {
        public bool UseManagedIdentity { get; set; }
        public string ManagedIdentityClientId { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public required string DatabaseName { get; set; }
        public required string LinksContainer { get; set; }
        public required string StatsContainer { get; set; }
    }
}
