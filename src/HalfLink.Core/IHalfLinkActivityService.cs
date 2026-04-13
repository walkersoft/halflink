namespace HalfLink.Core
{
    public record ClickActivityEvent(Guid LinkId, string Referrer, DateTime ClickedAt);

    public interface IHalfLinkActivityService
    {
        Task AddClickActivity(ClickActivityEvent clickActivityEvent);
    }
}
