using MassTransit;

namespace PostsModule.Application.Get;

public class GetPostConsumer : IConsumer<GetPostCommand>
{
	public async Task Consume(ConsumeContext<GetPostCommand> context)
	{
		var response = new GetPostResult();
		{
		};
		await context.RespondAsync(response);
	}
}
