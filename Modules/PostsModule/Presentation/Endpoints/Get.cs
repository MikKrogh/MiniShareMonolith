using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application;
using PostsModule.Application.Get;

namespace PostsModule.Presentation.Endpoints;

internal class Get
{
    internal static async Task<IResult> Process([FromServices] IRequestClient<GetPostCommand> client, [FromRoute] string postId)
    {
        try
        {
            var command = new GetPostCommand(postId);
            var clientResponse = await client.GetResponse<CommandResult<GetPostResult>>(command);

            if (clientResponse.Message.IsSuccess && clientResponse.Message.ResultValue is not null)
            {
                var commandResult = clientResponse.Message.ResultValue;
                var dto = new PostDto
                {
                    Id = commandResult.Id,
                    Title = commandResult.Title,
                    Description = commandResult.Description,
                    CreatorId = commandResult.CreatorId,
                    CreatorName = commandResult.CreatorName,
                    Images = commandResult.Images,
                    PrimaryColor = commandResult.PrimaryColor,
                    SecondaryColor = commandResult.SecondaryColor,
                };
                return Results.Ok(dto);
            }
            return Results.StatusCode(clientResponse.Message.ResultStatus);

        }
        catch (Exception)
        {
            return Results.Problem();
        }

    }
}
