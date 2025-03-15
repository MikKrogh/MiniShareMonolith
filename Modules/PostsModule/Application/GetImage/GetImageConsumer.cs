using MassTransit;
using PostsModule.Domain;
using System.IO;

namespace PostsModule.Application.GetImage;

public class GetImageConsumer : IConsumer<GetImageCommand>
{
    private readonly IImageStorageService imageService;

    public GetImageConsumer(IImageStorageService imageService)
    {
        this.imageService = imageService;
    }
    public async Task Consume(ConsumeContext<GetImageCommand> context)
    {
        try
        {
            var image = await imageService.GetImage(context.Message.PostId, context.Message.ImageId);
            if (image is null)
            {
                await context.RespondAsync(CommandResult<GetImageCommandResult>.NotFound());
                return;
            }

            MemoryStream memoryStream = null;
            using (memoryStream = new MemoryStream())
            {
                image.CopyTo(memoryStream);            
            }
            await context.RespondAsync(CommandResult<GetImageCommandResult>.Success(new() { File = memoryStream.ToArray() }));

        }
        catch (Exception)
        {
            await context.RespondAsync(CommandResult<GetImageCommandResult>.InternalError());
        }

    }
}
