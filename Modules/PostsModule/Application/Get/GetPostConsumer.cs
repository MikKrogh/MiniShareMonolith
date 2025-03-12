using MassTransit;
using PostsModule.Domain;

namespace PostsModule.Application.Get;

public class GetPostConsumer : IConsumer<GetPostCommand>
{
    private readonly IPostsRepository repository;

    public GetPostConsumer(IPostsRepository repository)
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

        var result = new GetPostResult()
        {
            Id = post.Id.ToString(),
            Faction = post.FactionName,
            Title = post.Title,
            Description = post.Description,
            CreatorName = post.CreatorName,
            CreatorId = post.CreatorId,
            Images = post.Images,
            PrimaryColor = post.PrimaryColour,
            SecondaryColor = post.SecondaryColour,
            CreationDate = post.CreationDate
        };

        var commandResult = CommandResult<GetPostResult>.Success(result);

        await context.RespondAsync(commandResult);
    }
}
