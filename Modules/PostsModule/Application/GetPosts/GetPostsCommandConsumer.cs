using MassTransit;

namespace PostsModule.Application.GetPosts;

public class GetPostsCommandConsumer : IConsumer<GetPostsCommand>
{
    public async Task Consume(ConsumeContext<GetPostsCommand> context)
    {
        var result = new GetPostsCommandResult();
        await context.RespondAsync(CommandResult<GetPostsCommandResult>.Success(result));
    }
}
