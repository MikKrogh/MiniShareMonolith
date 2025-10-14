
using Microsoft.AspNetCore.Mvc;

namespace EngagementModule.Notification;

internal static class GetNotifications
{ 
    public static async Task<IResult> Process([FromServices]ChainActivityService service, [FromQuery]string userId)
    {
        var postIds = await service.GetPostsWithNewComments(userId);

        var dto = postIds.Select(id => new NotifocationsDto { PostId = id })
            .ToList();
        return Results.Ok(dto);
    }
}

public class NotifocationsDto
{
    public string PostId { get; set; } = string.Empty;
    
}
