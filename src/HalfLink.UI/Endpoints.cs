using HalfLink.Core;
using HalfLink.Data;
using Microsoft.AspNetCore.Mvc;

namespace HalfLink.UI
{
    internal static class Endpoints
    {
        internal static void AddApplicationEndpoints(this WebApplication builder)
        {
            builder.MapGet(
                "/l/{halfLink}",
                async ([FromRoute] string halfLink, HalfLinkService halfLinkService, HalfLinkActivityQueue halfLinkActivityQueue, IHttpContextAccessor httpContextAccessor) =>
            {
                var link = await halfLinkService.GetLink(halfLink);
                if (link is null) return Results.Redirect("/not-found");

                var referrer = string.IsNullOrWhiteSpace(httpContextAccessor?.HttpContext?.Request.Headers.Referer)
                    ? "SYSTEM_UNKNOWN"
                    : $"{httpContextAccessor.HttpContext.Request.Headers.Referer}";

                await halfLinkActivityQueue.AddClickActivity(new ClickActivityEvent
                {
                    LinkId = link.Id,
                    Referrer = referrer,
                    ClickedAt = DateTime.UtcNow
                });

                return Results.Redirect(link.FullLink);
            });

            builder.MapPost(
                "/s/click",
                async ([FromBody] ClickActivityEvent clickActivityEvent, HalfLinkService halfLinkService) =>
            {
                await halfLinkService.CreateStat(new()
                {
                    Id = Guid.NewGuid(),
                    LinkId = clickActivityEvent.LinkId,
                    Referrer = clickActivityEvent.Referrer,
                    AccessedAt = clickActivityEvent.ClickedAt
                });
                return Results.Ok();
            });
        }
    }
}