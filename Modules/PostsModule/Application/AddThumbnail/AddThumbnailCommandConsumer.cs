
using PostsModule.Domain;

namespace PostsModule.Application.AddThumbnail;

public sealed class AddThumbnailCommandConsumer
{
    private readonly IImageStorageService _imageBlobService;
    private readonly ILogger<AddThumbnailCommandConsumer> logger;

    public AddThumbnailCommandConsumer(IImageStorageService imageBlobService,ILogger<AddThumbnailCommandConsumer> logger)
    {
        _imageBlobService = imageBlobService;
        this.logger = logger;
    }
    public async Task<CommandResult<AddThumbnailCommandResult>> Consume(AddThumbnailCommand context)
    {
        try
        {
            using (var stream = new MemoryStream(context.File))
            {
                await _imageBlobService.UploadThumbnail(stream, context.PostId);
            }
            var result = CommandResult<AddThumbnailCommandResult>.Success(null);
            return result;
        }
        catch (Exception e)
        {
            logger.LogError($"failed to create thumbnail for {context.PostId}.", e.Message);
            var result = CommandResult<AddThumbnailCommandResult>.InternalError();
            return result;
        }
    }
}
