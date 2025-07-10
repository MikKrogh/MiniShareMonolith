
using PostsModule.Domain;

namespace PostsModule.Application.GetPosts;

public class GetPostsCommandConsumer
{
    private readonly IPostsRepository repository;

    public GetPostsCommandConsumer(IPostsRepository repository)
    {
        this.repository = repository;
    }
    public async Task<CommandResult<GetPostsCommandResult>> Consume(GetPostsCommand context)
    {
        var posts = await repository.GetAll(
            context.QueryModel.Take,
            context.QueryModel.Descending,
            context.QueryModel.OrderBy,
            context.QueryModel.Filter,
            context.QueryModel.Search,
            context.QueryModel.Skip
        );

        var mappedPosts = posts.Items.Select(post => new PostDto()
        {
            Id = post.Id.ToString(),
            Title = post.Title,
            FactionName = post.FactionName,
            CreatorId = post.CreatorId.ToString(),
            Description = post.Description,
            PrimaryColor = post.PrimaryColor,
            SecondaryColor = post.SecondaryColor,
            CreationDate = post.CreationDate,
            CreatorName = post.CreatorName,
            Images = post.Images
        }).ToList();

        var result = new GetPostsCommandResult()
        {
            Posts = mappedPosts,
            TotalCount = posts.TotalCount
        };
        return CommandResult<GetPostsCommandResult>.Success(result);
    }
}
