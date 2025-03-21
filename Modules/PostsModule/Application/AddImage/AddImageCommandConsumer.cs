using MassTransit;
using PostsModule.Domain;
using PostsModule.Domain.Auth;
using PostsModule.Presentation.Endpoints;

namespace PostsModule.Application.AddImage;

public sealed class AddImageCommandConsumer : IConsumer<AddImageCommand>
{    
    private readonly IImageStorageService _imageBlobService;
    private readonly IImageRepository _imageRepository;

    public AddImageCommandConsumer(IImageStorageService imageService, IImageRepository imageRepository )
    {        
        this._imageBlobService = imageService;
        this._imageRepository = imageRepository;
    }

    public async Task Consume(ConsumeContext<AddImageCommand> context)
    {
        if (!IsValid(context.Message))
        {
            var result = CommandResult<AddImageCommandResult>.FailedToValidate();
            await context.RespondAsync(result);
        }
        else
        {
            Stream? stream = StreamBank.GetStream(context.Message.PostId,context.Message.StreamId);
            if (stream is null)
            {
                var result = CommandResult<AddImageCommandResult>.InternalError();
                await context.RespondAsync(result);
                return;
            }

            try
            {
                string newFileName = context.Message.StreamId.ToString() + context.Message.FileExtension;

                await _imageBlobService.UploadImage(stream, context.Message.PostId.ToString(), newFileName);
                await _imageRepository.Create(Image.Create(newFileName, context.Message.PostId.ToString()));

                await context.RespondAsync(CommandResult<AddImageCommandResult>.Success(null));
            }
            catch (Exception ex)
            {
                await context.RespondAsync(CommandResult<AddImageCommandResult>.InternalError());
            }
        }
    }

    private bool IsValid(AddImageCommand command)
    {
        bool isValidExtension = IsValidFileExtension(command.FileExtension);

        return isValidExtension; 
    }

    private bool IsValidFileExtension(string fileExtension)
    {
        return fileExtension == ".jpg" ||
               fileExtension == ".jpeg" ||
               fileExtension == ".bmp" ||
               fileExtension == ".png";

    }
}
