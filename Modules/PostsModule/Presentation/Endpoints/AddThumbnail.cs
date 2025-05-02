using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PostsModule.Application;
using PostsModule.Application.AddImage;
using PostsModule.Application.AddThumbnail;
using PostsModule.Application.Create;
using PostsModule.Domain.Auth;

namespace PostsModule.Presentation.Endpoints;

public class AddThumbnail
{
    [RequestFormLimits(MultipartBodyLengthLimit = 500_000)]
    internal static async Task<IResult> Process(IFormFile file, [FromServices] IAuthHelper authHelper, IRequestClient<AddThumbnailCommand> client,ILogger<AddThumbnail> logger,[FromRoute] string postId, [FromQuery]string token)
    {
        var claims = authHelper.ReadClaims(token);
        if (claims == null || !claims.Any() || claims["postId"] != postId.ToString())
        {
            logger.LogError("request for AddImage called with bad token: {0} ", token);
            return Results.Problem("bad token");
        }
        if (!IsValidFileFormat(Path.GetExtension(file.FileName)))
        {
            return Results.BadRequest();
        } 


        byte[] fileBytes;
        
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
    private static bool IsValidFileFormat(string format)
    {
        int isEqual = string.Compare(format, ".webm", StringComparison.OrdinalIgnoreCase);
        return isEqual == 0;
    }

}
