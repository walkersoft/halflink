namespace HalfLink.Data
{
    public class CosmosSettings
    {
        public string Endpoint { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string LinksContainer { get; set; } = string.Empty;
        public string StatsContainer { get; set; } = string.Empty;
    }
}
