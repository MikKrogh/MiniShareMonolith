using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application;
using PostsModule.Application.Create;
using PostsModule.Application.DeletePost;

namespace PostsModule.Presentation.Endpoints;

public static class DeletePost
{
    public static async Task<IResult> Process([FromServices] IRequestClient<DeletionRequestedCommand> client, [FromRoute]string postId, [FromQuery]string userId)
    {
        if (string.IsNullOrEmpty(postId) || string.IsNullOrEmpty(userId))        
            return Results.BadRequest("PostId and UserId cannot be null or empty.");

        var command = new DeletionRequestedCommand()
        {
            PostId = postId,
            UserId = userId
        };
        var result = await client.GetResponse<CommandResult<DeletionRequestedCommandResult>>(command);
        return Results.Ok();

    }
}
