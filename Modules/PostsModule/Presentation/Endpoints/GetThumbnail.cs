using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application;
using PostsModule.Application.GetImage;

namespace PostsModule.Presentation.Endpoints;

public class GetThumbnail
{
    internal static async Task<IResult> Process([FromServices] IRequestClient<GetImageCommand> client, [FromRoute] string postId )
    {
        var command = new GetImageCommand("thumbnails",postId);
        var commandResult = await client.GetResponse<CommandResult<GetImageCommandResult>>(command);
        var result = commandResult.Message;

        if (result.IsSuccess && result.ResultValue is not null)
        {
            return Results.File(result.ResultValue.File);
        }
        return Results.StatusCode(result.ResultStatus);
    }
}
