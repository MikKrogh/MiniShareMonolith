using Microsoft.AspNetCore.Mvc;
using PostsModule.Application.Create;
using PostsModule.Infrastructure;
using System.Diagnostics.Metrics;

namespace PostsModule.Presentation.Endpoints;
public class CreatePost
{
    public static async Task<IResult> Process(ILogger<CreatePost>? logger,[FromServices] CreatePostCommandConsumer client,[FromServices] IPresignedUrlGenerator pug, [FromBody] CreateBody body, [FromQuery]string UserId)    
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
            
            var commandResult = await client.Consume(command);

            if (commandResult is not null && commandResult.IsSuccess && commandResult.ResultValue is not null)
            {
                var presignedUrls = await pug.GetPresignedUris(commandResult.ResultValue.PostId, 8);
                return Results.Ok(new SuccessResponse
                {
                    PostId = commandResult.ResultValue.PostId,
                    PresignedUrls = presignedUrls
                });
            }
            logger.LogError("Error creating post by creatorId {0}.", UserId);
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
    public IEnumerable<string> PresignedUrls { get; set; }
}

public class PostCreatedMeter
{
    public static readonly Meter Meter = new Meter("PostModule.PostCreated", "1.0.0");
    public static string MeterName => Meter.Name;
    public static readonly Counter<int> PostCreatedCounter = Meter.CreateCounter<int>("post_created_count");
}