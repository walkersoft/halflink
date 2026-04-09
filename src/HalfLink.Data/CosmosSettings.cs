namespace HalfLink.Data
{
    public class CosmosSettings
    {
        public bool UseManagedIdentity { get; set; }
        public string Endpoint { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public required string DatabaseName { get; set; }
        public required string LinksContainer { get; set; }
        public required string StatsContainer { get; set; }
    }
}
