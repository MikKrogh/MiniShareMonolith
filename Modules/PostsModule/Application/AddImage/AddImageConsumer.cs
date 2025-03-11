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
        if (!IsValid(context.Message))
        {
            var result = CommandResult.FailedToValidate();
            await context.RespondAsync(result);
        }
        else
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

    private bool IsValid(AddImageCommand command)
    {
        bool isValidExtension = IsValidFileExtension(command.FileExtension);
        bool postIdIsGuid = Guid.TryParse(command.PostId, out Guid _);
        bool streamExists = StreamBank.ContainsStream(command.StreamId);

        return isValidExtension && postIdIsGuid && streamExists;
    }

    private bool IsValidFileExtension(string fileExtension)
    {
        return fileExtension == ".jpg" ||
               fileExtension == ".jpeg" ||
               fileExtension == ".bmp" ||
               fileExtension == ".png";

    }
}
