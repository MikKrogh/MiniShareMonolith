using MassTransit;
using PostsModule.Domain;

namespace PostsModule.Application.AddThumbnail;

public sealed class AddThumbnailCommandConsumer : IConsumer<AddThumbnailCommand>
{
    private readonly IImageStorageService _imageBlobService;
    private readonly ILogger<AddThumbnailCommandConsumer> logger;

    public AddThumbnailCommandConsumer(IImageStorageService imageBlobService,ILogger<AddThumbnailCommandConsumer> logger)
    {
        _imageBlobService = imageBlobService;
        this.logger = logger;
    }
    public async Task Consume(ConsumeContext<AddThumbnailCommand> context)
    {
        try
        {
            using (var stream = new MemoryStream(context.Message.File))
            {
                await _imageBlobService.UploadThumbnail(stream, context.Message.PostId);
            }
            var result = CommandResult<AddThumbnailCommandResult>.Success(null);
            await context.RespondAsync(result);
        }
        catch (Exception e)
        {
            logger.LogError($"failed to create thumbnail for {context.Message.PostId}.", e.Message);
            var result = CommandResult<AddThumbnailCommand>.InternalError();
            await context.RespondAsync(result);
        }
    }
}
