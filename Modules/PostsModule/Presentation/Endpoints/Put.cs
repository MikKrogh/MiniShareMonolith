using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application;
using PostsModule.Application.AddImage;
using System.Collections.Concurrent;
namespace PostsModule.Presentation.Endpoints;

internal class Put
{
    internal static async Task<IResult> ProcessAddImage( IFormFile file, [FromServices] IRequestClient<AddImageCommand> client, [FromRoute] string postId, [FromQuery] string token)    
    {
        var t = StreamBank.RegisterStream(file.OpenReadStream());

        var command = new AddImageCommand()
        {
            FileExtension = Path.GetExtension(file.FileName),
            PostId = postId,
            StreamId = t
        };

        var result = await client.GetResponse<CommandResult>(command);

        if (result.Message.IsSuccess)
            return Results.Ok();
        return Results.Problem();
    }
}


public static class StreamBank
{
    private static readonly ConcurrentDictionary<Guid, Stream> Streams = new();

    public static Guid RegisterStream(Stream stream)
    {
        var id = Guid.NewGuid();
        Streams[id] = stream;
        return id;
    }

    public static Stream? GetStream(Guid id)
    {
        return Streams.TryGetValue(id, out var stream) ? stream : null;
    }
    public static void RemoveStream(Guid id)
    {
        if (Streams.TryRemove(id, out var stream))
        {
            stream.Dispose();
        }
    }
}