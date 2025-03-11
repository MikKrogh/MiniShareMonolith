using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application;
using PostsModule.Application.AddImage;
using System.Collections.Concurrent;
using System.Collections.Specialized;
namespace PostsModule.Presentation.Endpoints;

internal class Put
{
    [RequestFormLimits(MultipartBodyLengthLimit = 9_000_000)]
    internal static async Task<IResult> ProcessAddImage( IFormFile file, [FromServices] IRequestClient<AddImageCommand> client, [FromRoute] string postId, [FromQuery] string token)    
    {
        var imageId = StreamBank.RegisterStream(Guid.Parse(postId),file.OpenReadStream());
        if (imageId == null)
            return Results.BadRequest();

        var command = new AddImageCommand()
        {
            FileExtension = Path.GetExtension(file.FileName),
            PostId = postId,
            StreamId = imageId.Value
        };

        var result = await client.GetResponse<CommandResult>(command);

        if (result.Message.IsSuccess)
            return Results.Ok();
        return Results.StatusCode(result.Message.ResultStatus);
    }
}

//MassTransits message broker seralizes the messages between command creator and command handler, and the stream is not serializable.
public static class StreamBank
{
    private static readonly ConcurrentDictionary<Guid, ImageContainer> Streams = new(); // jwtToken is the Key

    public static Guid? RegisterStream(Guid postId,Stream stream)
    {
        if (!Streams.ContainsKey(postId))        
            Streams.TryAdd(postId, new ImageContainer());        

        try
        {
            Guid imageId = Guid.NewGuid();
            var success = Streams[postId].TryAddStream(imageId,stream);
            return success 
                ? imageId 
                : null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public static Stream? GetStream(Guid postId, Guid ImageId)
    {
        if (Streams.ContainsKey(postId))
        {
            return Streams[postId].GetStream(ImageId);            
        }
        return null;
    }

    private class ImageContainer
    {
        private int MaxStreams = 8;
        public int StreamsHandedOver { get; private set; } = 0;
        private List<(Guid imageId ,Stream stream)> Streams { get; init; } = new();

        public bool TryAddStream(Guid imageId, Stream stream)
        {
            if (StreamsHandedOver <= MaxStreams)
            {
                Streams.Add((imageId, stream));
                StreamsHandedOver++;
                return true;
            }
            return false;
        }

        public Stream GetStream(Guid imageId)
        {
            var stream = Streams.FirstOrDefault(x => x.imageId == imageId);
            return stream.stream;

        }
        public void RemoveStream(Guid imageId) => Streams.Remove(Streams.First(x => x.imageId == imageId));      
        
    }
}