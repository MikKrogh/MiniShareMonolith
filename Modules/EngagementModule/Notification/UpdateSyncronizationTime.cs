using Microsoft.AspNetCore.Mvc;

namespace EngagementModule.Notification;

internal static class UpdateSyncronizationTime
{
    public static async Task<IResult> Process([FromServices] ChainActivityService service, [FromQuery] string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return Results.BadRequest("UserId cannot be null or empty.");
        }
        await service.UpdateLastSync(userId);
        return Results.Ok();
    }
}
