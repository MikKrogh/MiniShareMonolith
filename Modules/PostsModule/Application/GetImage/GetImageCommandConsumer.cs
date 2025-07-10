
using PostsModule.Domain;


namespace PostsModule.Application.GetImage;

public sealed class GetImageCommandConsumer
{
    private readonly IImageStorageService imageService;

    public GetImageCommandConsumer(IImageStorageService imageService)
    {
        this.imageService = imageService;
    }
    public async Task<CommandResult<GetImageCommandResult>> Consume(GetImageCommand context)
    {
        try
        {
            var image = await imageService.GetImage(context.PostId, context.ImageId);
            if (image is null)
            {
                return CommandResult<GetImageCommandResult>.NotFound();
                
            }

            MemoryStream memoryStream;
            using (memoryStream = new MemoryStream())
            {
                image.CopyTo(memoryStream);
            }
            return  CommandResult<GetImageCommandResult>.Success(new() { File = memoryStream.ToArray() });

        }
        catch (Exception)
        {
            return CommandResult<GetImageCommandResult>.InternalError();
        }

    }
}
