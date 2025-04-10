using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application;
using PostsModule.Application.Create;
using PostsModule.Domain.Auth;

namespace PostsModule.Presentation.Endpoints;
internal class CreatePost
{
    internal static async Task<IResult> Process(ILogger<CreatePost>? logger,[FromServices] IRequestClient<CreatePostCommand> client, IAuthHelper auth, [FromBody] CreateBody body)
    {
        if (logger is null)
        {
            return Results.Problem("Logger is null");
        }
        logger.LogError("hello");
        logger?.LogInformation("Creating post with title {title} and creatorId {creatorId}", body.Title, body.CreatorId);
        var command = new CreatePostCommand()
        {
            Title = body.Title,
            FactionName = body.FactionName,
            Description = body.Description,
            CreatorId = body.CreatorId,
            PrimaryColor = string.IsNullOrEmpty(body.PrimaryColor) ? string.Empty : body.PrimaryColor,
            SecondaryColor = string.IsNullOrEmpty(body.SecondaryColor) ? string.Empty : body.SecondaryColor,
        };

        try
        {
            var clientResponse = await client.GetResponse<CommandResult<CreatePostCommandResult>>(command);
            var commandResult = clientResponse.Message;

            if (clientResponse.Message.IsSuccess && commandResult is not null)
            {
                var token = auth.CreateToken(
                    DateTime.UtcNow.AddMinutes(5),
                    ClaimValueHolder.Create("postId", commandResult.ResultValue.PostId)
                );

                return Results.Ok(new SuccessRespnse
                {
                    PostId = commandResult.ResultValue.PostId,
                    Token = token
                });
            }
            return Results.StatusCode(409);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating post with title {title} and creatorId {creatorId}", body.Title, body.CreatorId);
            return Results.Problem(ex.Message);
        }
    }
}
public class SuccessRespnse
{
    public string PostId { get; set; }
    public string Token { get; set; }
}

