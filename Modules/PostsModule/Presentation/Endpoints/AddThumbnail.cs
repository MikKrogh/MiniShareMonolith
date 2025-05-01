using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application;
using PostsModule.Application.AddImage;
using PostsModule.Application.AddThumbnail;
using PostsModule.Application.Create;
using PostsModule.Domain.Auth;

namespace PostsModule.Presentation.Endpoints;

public class AddThumbnail
{
    //internal static async Task<IResult> Process(IFormFile file, [FromServices] IAuthHelper authHelper, ILogger<AddImage> logger, [FromServices] IRequestClient<AddImageCommand> client, [FromRoute] Guid postId, [FromQuery] string token)
    internal static async Task<IResult> Process(IFormFile file, [FromServices] IRequestClient<AddThumbnailCommand> client,[FromRoute] string postId)
    {
        byte[] fileBytes;
        string t = "";
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            fileBytes = memoryStream.ToArray();
        }
        var command = new AddThumbnailCommand()
        {
            PostId = postId,
            File = fileBytes

        };
        var result = await client.GetResponse<CommandResult<AddThumbnailCommandResult>>(command);
        if (result.Message.IsSuccess)
            return Results.Ok();
        return Results.StatusCode(result.Message.ResultStatus);


    }

}
