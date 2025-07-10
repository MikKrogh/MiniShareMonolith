using Microsoft.AspNetCore.Mvc;
using PostsModule.Application.GetImage;

namespace PostsModule.Presentation.Endpoints;

public class GetThumbnail
{
    internal static async Task<IResult> Process([FromServices] GetImageCommandConsumer client, [FromRoute] string postId )
    {
        var command = new GetImageCommand("thumbnails",postId);
        var commandResult = await client.Consume(command);        

        if (commandResult.IsSuccess && commandResult.ResultValue is not null)
        {
            return Results.File(commandResult.ResultValue.File);
        }
        return Results.StatusCode(commandResult.ResultStatus);
    }
}
