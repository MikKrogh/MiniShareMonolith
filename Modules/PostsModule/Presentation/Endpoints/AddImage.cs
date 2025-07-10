using Microsoft.AspNetCore.Mvc;
using PostsModule.Application.AddImage;
using PostsModule.Domain.Auth;
using System.Collections.Concurrent;

namespace PostsModule.Presentation.Endpoints;

internal class AddImage
{
    [RequestFormLimits(MultipartBodyLengthLimit = 9_000_000)]
    internal static async Task<IResult> Process(IFormFile file, [FromServices] IAuthHelper authHelper, ILogger<AddImage> logger, [FromServices] AddImageCommandConsumer client, [FromRoute] Guid postId, [FromQuery] string token)
    {
        var claims = authHelper.ReadClaims(token);
        if (claims == null || !claims.Any() || claims["postId"] != postId.ToString()) 
        {
            logger.LogError("request for AddImage called with bad token: {0} ", token);
            return Results.Problem("bad token");        
        }

        var command = new AddImageCommand()
        {
            FileExtension = Path.GetExtension(file.FileName),
            PostId = postId,
            StreamId = Guid.NewGuid()
        };

        long ticks;
        long.TryParse(claims["exp"], out ticks);
        DateTime expiration = DateTimeOffset.FromUnixTimeSeconds(ticks).LocalDateTime;

        var couldAdd = StreamBank.RegisterStream(command.PostId, command.StreamId, file.OpenReadStream(), expiration);
        if (!couldAdd)
        {
            logger.LogError("failed to append imagestream for token: {0}", token);
            return Results.Problem();
        }

        var result = await client.Consume(command);

        if (result.IsSuccess)
            return Results.Ok();
        return Results.StatusCode(result.ResultStatus);
    }
}

//MassTransits messageBroker seralizes the messages between command-creator and command-handler,
//To avoid reading the stream twice, i use this bank, and then parse an id around instead.
//TODO:Look into better ways to handle this, maybe a better way to handle the streams, or a better way to handle the messages.
public static class StreamBank
{
    public static List<string> Cleanings = new();
    private static readonly ConcurrentDictionary<Guid, ImageState> StreamsByPost = new();
    private static readonly Timer CleanupTimer = new Timer(_ =>
        RemoveExpiredStates(),
        null,
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(5));



    public static bool RegisterStream(Guid postId, Guid streamId, Stream stream, DateTime ex)
    {
        var postStreams = StreamsByPost.GetOrAdd(postId, _ => new(postId, ex));
        return postStreams.Add(streamId, stream);
    }
    public static Stream? GetStream(Guid postId, Guid streamId)
    {
        return StreamsByPost.FirstOrDefault(x => x.Key == postId).Value.Get(streamId);

    }

    public static void RemoveStream(Guid postId, Guid imageId)
    {
        if (StreamsByPost.TryGetValue(postId, out ImageState? postStreams))
        {
            postStreams.Remove(imageId);
        }
    }

    private static void RemoveExpiredStates()
    {
        DateTime currentTime = DateTime.UtcNow;
        var pairs = StreamsByPost.Select(x => (x.Value.PostId, x.Value.ExpirationDate)).ToList();
        foreach (var item in pairs)
        {
            if (item.ExpirationDate.AddMinutes(1) > currentTime)
            {
                StreamsByPost.Remove(item.PostId, out _);
            }
        }
    }

    private class ImageState
    {
        private const int MaxUploads = 8;
        public int ImageUploadsAttempsCount { get; set; } = 0;
        public Guid PostId { get; set; }
        public DateTime ExpirationDate { get; set; } = new();
        public List<(Guid imageId, Stream stream)> Streams { get; } = new();
        private object _lock = new();
        public ImageState(Guid postId, DateTime expiraiton)
        {
            PostId = postId;
            ExpirationDate = expiraiton;
        }
        public Stream Get(Guid ImageId)
        {
            var tuple = Streams.FirstOrDefault(x => x.imageId == ImageId);
            return tuple.stream;

        }
        public bool Add(Guid imageId, Stream stream)
        {
            lock (_lock)
            {
                if (this.ImageUploadsAttempsCount < MaxUploads)
                {
                    this.ImageUploadsAttempsCount++;
                    this.Streams.Add((imageId, stream));
                    return true;
                }
                return false;
            }
        }
        public void Remove(Guid imageId)
        {
            lock (_lock)
            {
                try
                {
                    Streams.Remove(Streams.First(x => x.imageId == imageId));
                }
                catch (Exception)
                {
                }
            }
        }

    }
}