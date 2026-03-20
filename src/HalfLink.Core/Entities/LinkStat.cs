namespace HalfLink.Core.Entities
{
    internal class LinkStat
    {
        public Guid Id { get; set; }
        public Guid LinkId { get; set; }
        public string Referrer { get; set; } = string.Empty;
        public DateTime AccessedAt { get; set; }
    }
}
