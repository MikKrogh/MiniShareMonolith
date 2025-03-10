using MassTransit;
using PostsModule.Domain;
using PostsModule.Domain.Auth;
using PostsModule.Presentation.Endpoints;

namespace PostsModule.Application.AddImage;

public class AddImageConsumer : IConsumer<AddImageCommand>
{
    private readonly IAuthHelper _authHelper;
    private readonly IImageStorageService _imageBlobService;
    private readonly IImageRepository _imageRepository;
    private readonly ILogger<AddImageConsumer> _logger;

    public AddImageConsumer(IAuthHelper authHelper, IImageStorageService imageService, IImageRepository imageRepository, ILogger<AddImageConsumer> logger)
    {
        this._authHelper = authHelper;
        this._imageBlobService = imageService;
        this._imageRepository = imageRepository;
        this._logger = logger;
    }

    public async Task Consume(ConsumeContext<AddImageCommand> context)
    {
        var stream = StreamBank.GetStream(context.Message.StreamId);

        try
        {
            string newFileName = DateTime.Now.Ticks.ToString();

            await _imageBlobService.UploadImage(stream, context.Message.PostId, newFileName, context.Message.FileExtension);
            await _imageRepository.Create(Image.Create(newFileName, context.Message.PostId));

            await context.RespondAsync(CommandResult.Success());
        }
        catch (Exception ex)
        {
            await context.RespondAsync(CommandResult.InternalError());
        }
    }
}
