using HalfLink.Core;

namespace HalfLink.UI
{
    internal static class Endpoints
    {
        internal static void AddApplicationEndpoints(this WebApplication builder)
        {
            builder.MapGet("/l/{halfLink}", async (string halfLink, HalfLinkService service) =>
            {
                var link = await service.GetLink(halfLink);
                if (link is null) return Results.Redirect("/not-found");

                return Results.Redirect(link.FullLink);
            });
        }
    }
}
