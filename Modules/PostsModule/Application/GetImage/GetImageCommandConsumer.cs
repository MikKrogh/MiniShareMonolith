using MassTransit;
using PostsModule.Domain;


namespace PostsModule.Application.GetImage;

public sealed class GetImageCommandConsumer : IConsumer<GetImageCommand>
{
    private readonly IImageStorageService imageService;

    public GetImageCommandConsumer(IImageStorageService imageService)
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

            MemoryStream memoryStream;
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
