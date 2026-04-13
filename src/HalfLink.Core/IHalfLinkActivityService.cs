using System.Diagnostics.CodeAnalysis;

namespace HalfLink.Core
{
    [ExcludeFromCodeCoverage]
    public record ClickActivityEvent(Guid LinkId, string Referrer, DateTime ClickedAt);

    public interface IHalfLinkActivityService
    {
        Task AddClickActivity(ClickActivityEvent clickActivityEvent);
    }
}
