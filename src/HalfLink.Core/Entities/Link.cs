namespace HalfLink.Core.Entities
{
    public class Link
    {
        public required Guid Id { get; set; }
        public string? Description { get; set; }
        public required string FullLink { get; set; }
        public required string HalfLink { get; set; }
    }
}
