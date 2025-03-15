using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application;
using PostsModule.Application.AddImage;
using PostsModule.Domain.Auth;

using System.Collections.Concurrent;
using System.Linq;

namespace PostsModule.Presentation.Endpoints;

internal class Put
{
    [RequestFormLimits(MultipartBodyLengthLimit = 9_000_000)]
    internal static async Task<IResult> ProcessAddImage( IFormFile file,[FromServices] IAuthHelper authHelper,[FromServices] IRequestClient<AddImageCommand> client, [FromRoute] Guid postId, [FromQuery] string token)    
    {
        var claims = authHelper.ReadClaims(token);
        if (claims == null || !claims.Any() || claims["postId"] != postId.ToString()) return Results.Problem();

        var t = claims["postId"];

        var command = new AddImageCommand()
        {
            FileExtension = Path.GetExtension(file.FileName),
            PostId = postId,
            StreamId = Guid.NewGuid()
        };

        StreamBank.RegisterStream(command.PostId,command.StreamId, file.OpenReadStream());

        var result = await client.GetResponse<CommandResult<AddImageCommandResult>>(command);

        
        //Add a subscriber to clear the streambank by post, after the token has expired
        if (result.Message.IsSuccess)
            return Results.Ok();
        return Results.StatusCode(result.Message.ResultStatus);
    }
}

//MassTransits messageBroker seralizes the messages between command-creator and command-handler,
//To avoid reading the stream twice, i use this bank, and then parse an id around instead.
//TODO:Look into better ways to handle this, maybe a better way to handle the streams, or a better way to handle the messages.
public static class StreamBank
{
    private const int MaxStreamsPerPost = 8;    
    
    public static readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, Stream>> StreamsByPost = new();

    public static bool RegisterStream(Guid postId, Guid streamId,Stream stream )
    {
        var postStreams = StreamsByPost.GetOrAdd(postId, _ => new ConcurrentDictionary<Guid, Stream>());

        if (MaxStreamsPerPost <= postStreams.Count ) return false;       
        return postStreams.TryAdd(streamId, stream);
    }
    public static Stream? GetStream(Guid postId, Guid streamId)
    {
        return StreamsByPost.TryGetValue(postId, out var postStreams)
            && postStreams.TryGetValue(streamId, out var stream)
            ? stream
            : null;
    }

    public static void RemoveStream(Guid postId, Guid streamId)
    {
        if (StreamsByPost.TryGetValue(postId, out var postStreams))
        {
            postStreams.TryRemove(streamId, out _);

        }
    }
}