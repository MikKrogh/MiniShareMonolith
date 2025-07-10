using Microsoft.AspNetCore.Mvc;
using PostsModule.Application;
using PostsModule.Application.Get;

namespace PostsModule.Presentation.Endpoints;

internal class GetPost
{
    internal static async Task<IResult> Process([FromServices] GetPostCommandConsumer client, [FromRoute] string postId)
    {
        try
        {
            var command = new GetPostCommand(postId);
            var clientResponse = await client.Consume(command);

            if (clientResponse.IsSuccess && clientResponse.ResultValue is not null)
            {
                var commandResult = clientResponse.ResultValue;
                var dto = new PostDto
                {
                    Id = commandResult.Id,
                    Title = commandResult.Title,
                    Description = commandResult.Description,
                    CreatorId = commandResult.CreatorId,
                    CreatorName = commandResult.CreatorName,
                    CreationDate = commandResult.CreationDate,
                    Images = commandResult.Images,
                    PrimaryColor = commandResult.PrimaryColor,
                    SecondaryColor = commandResult.SecondaryColor,
                };
                return Results.Ok(dto);
            }
            return Results.StatusCode(clientResponse.ResultStatus);

        }
        catch (Exception)
        {
            return Results.Problem();
        }

    }
}
