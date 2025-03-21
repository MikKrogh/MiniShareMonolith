using MassTransit;
using PostsModule.Domain;

namespace PostsModule.Application.Get;

public sealed class GetPostCommandConsumer : IConsumer<GetPostCommand>
{
    private readonly IPostsRepository repository;

    public GetPostCommandConsumer(IPostsRepository repository)
    {
        this.repository = repository;
    }
    public async Task Consume(ConsumeContext<GetPostCommand> context)
    {
        var post = await repository.Get(context.Message.PostId);
        if (post is null)
        {
            await context.RespondAsync(default);
            return;
        }

        var result = new GetPostCommandResult()
        {
            Id = post.Id.ToString(),
            Faction = post.FactionName,
            Title = post.Title,
            Description = post.Description,
            CreatorName = post.CreatorName,
            CreatorId = post.CreatorId,
            Images = post.Images,
            PrimaryColor = post.PrimaryColor,
            SecondaryColor = post.SecondaryColor,
            CreationDate = post.CreationDate
        };

        var commandResult = CommandResult<GetPostCommandResult>.Success(result);

        await context.RespondAsync(commandResult);
    }
}
