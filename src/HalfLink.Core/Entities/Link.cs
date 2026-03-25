namespace HalfLink.Core.Entities
{
    public class Link
    {
        public Guid Id { get; set; }
        public string FullLink { get; set; } = string.Empty;
        public string HalfLink { get; set; } = string.Empty;
    }
}
