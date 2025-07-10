
using PostsModule.Domain;
using PostsModule.Presentation.Endpoints;

namespace PostsModule.Application.AddImage;

public sealed class AddImageCommandConsumer
{
    private readonly IImageStorageService _imageBlobService;
    private readonly IImageRepository _imageRepository;

    public AddImageCommandConsumer(IImageStorageService imageService, IImageRepository imageRepository)
    {
        this._imageBlobService = imageService;
        this._imageRepository = imageRepository;
    }

    public async Task<CommandResult<AddImageCommandResult>> Consume(AddImageCommand context)
    {
        if (!IsValid(context))
        {
            var result = CommandResult<AddImageCommandResult>.FailedToValidate();
            return result;
        }
        else
        {
            Stream? stream = StreamBank.GetStream(context.PostId, context.StreamId);
            if (stream is null)
            {
                var result = CommandResult<AddImageCommandResult>.InternalError();
                return result;
            }

            try
            {
                string newFileName = context.StreamId.ToString() + context.FileExtension;

                await _imageBlobService.UploadImage(stream, context.PostId.ToString(), newFileName);
                await _imageRepository.Create(Image.Create(newFileName, context.PostId.ToString()));

                StreamBank.RemoveStream(context.PostId, context.StreamId);
                return CommandResult<AddImageCommandResult>.Success(null);
            }
            catch (Exception ex)
            {
                return CommandResult<AddImageCommandResult>.InternalError();
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
               fileExtension == ".webp" ||
               fileExtension == ".png";

    }
}
