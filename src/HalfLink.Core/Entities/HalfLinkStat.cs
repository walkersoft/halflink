namespace HalfLink.Core.Entities
{
    public class HalfLinkStat
    {
        public Guid Id { get; set; }
        public Guid LinkId { get; set; }
        public string Referrer { get; set; } = string.Empty;
        public DateTime AccessedAt { get; set; }
    }
}
