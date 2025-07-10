
using PostsModule.Domain;

namespace PostsModule.Application.Get;

public sealed class GetPostCommandConsumer
{
    private readonly IPostsRepository repository;

    public GetPostCommandConsumer(IPostsRepository repository)
    {
        this.repository = repository;
    }
    public async Task<CommandResult<GetPostCommandResult>> Consume(GetPostCommand context)
    {
        var post = await repository.Get(context.PostId);
        if (post is null)
        {
            return CommandResult<GetPostCommandResult>.NotFound();            
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

        return commandResult;
    }
}
