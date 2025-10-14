using Microsoft.AspNetCore.Mvc;
using PostsModule.Application.AddThumbnail;
using PostsModule.Domain.Auth;

namespace PostsModule.Presentation.Endpoints;

public class AddThumbnail
{
    [RequestFormLimits(MultipartBodyLengthLimit = 500_000)]
    internal static async Task<IResult> Process(IFormFile file, AddThumbnailCommandConsumer client,ILogger<AddThumbnail> logger,[FromRoute] string postId, [FromQuery]string token)
    {

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
        var result = await client.Consume(command);
        if (result.IsSuccess)
            return Results.Ok();
        return Results.StatusCode(result.ResultStatus);


    }
    private static bool IsValidFileFormat(string format)
    {
        int isEqual = string.Compare(format, ".webp", StringComparison.OrdinalIgnoreCase);
        return isEqual == 0;
    }

}
