using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application;
using PostsModule.Application.GetImage;

namespace PostsModule.Presentation.Endpoints;

internal class GetImage
{
    internal static async Task<IResult> Process([FromServices] IRequestClient<GetImageCommand> client, [FromRoute] string postId, [FromRoute]string ImageId)
    {
        var command = new GetImageCommand(postId,  ImageId);
        var commandResult = await client.GetResponse<CommandResult<GetImageCommandResult>>(command);
        var result = commandResult.Message;

        if (result.IsSuccess && result.ResultValue is not null)
        {
            return Results.File(result.ResultValue.File);
        }
        return Results.StatusCode(result.ResultStatus);
    }
}