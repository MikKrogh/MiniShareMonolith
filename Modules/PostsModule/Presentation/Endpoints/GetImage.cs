using Microsoft.AspNetCore.Mvc;
using PostsModule.Application.GetImage;

namespace PostsModule.Presentation.Endpoints;

internal class GetImage
{
    internal static async Task<IResult> Process([FromServices] GetImageCommandConsumer client, [FromRoute] string postId, [FromRoute] string ImageId)
    {
        var command = new GetImageCommand(postId, ImageId);
        var commandResult = await client.Consume(command);
        

        if (commandResult.IsSuccess && commandResult.ResultValue is not null)
        {
            return Results.File(commandResult.ResultValue.File);
        }
        return Results.StatusCode(commandResult.ResultStatus);
    }
}