using MassTransit;
namespace PostsModule.Application.Create;

public class CreatePostConsumer : IConsumer<CreatePostCommand>
{
	public async Task Consume(ConsumeContext<CreatePostCommand> context)
	{

		var response = new CreatePostResult 
		{
			PostId = Guid.NewGuid().ToString()
		};
		await context.RespondAsync(response);
	}
}
