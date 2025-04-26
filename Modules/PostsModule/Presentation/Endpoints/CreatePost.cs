using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application;
using PostsModule.Application.Create;
using PostsModule.Domain.Auth;
using System.Diagnostics.Metrics;

namespace PostsModule.Presentation.Endpoints;
public class CreatePost
{
    public static async Task<IResult> Process(ILogger<CreatePost>? logger,[FromServices] IRequestClient<CreatePostCommand> client, IAuthHelper auth, [FromBody] CreateBody body, [FromQuery]string UserId)    
    {               
        var command = new CreatePostCommand()
        {
            Title = body.Title,
            FactionName = body.FactionName,
            Description = body.Description,
            CreatorId = UserId,
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

                logger.LogInformation("Post created by creatorId {0}", UserId);
                PostCreatedMeter.PostCreatedCounter.Add(1, new KeyValuePair<string, object?>("post_creator", UserId));
                return Results.Ok(new SuccessResponse
                {
                    PostId = commandResult.ResultValue.PostId,
                    Token = token
                });
            }
            logger.LogError("No Exception. Error creating post by creatorId {0}.", UserId);
            return Results.StatusCode(commandResult.ResultStatus);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating post by creatorId {0}", UserId);
            return Results.Problem(ex.Message);
        }
    }
}
public class SuccessResponse
{
    public string PostId { get; set; }
    public string Token { get; set; }
}

public class PostCreatedMeter
{
    public static readonly Meter Meter = new Meter("PostModule.PostCreated", "1.0.0");
    public static string MeterName => Meter.Name;
    public static readonly Counter<int> PostCreatedCounter = Meter.CreateCounter<int>("post_created_count");
}