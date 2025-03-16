using MassTransit;
using PostsModule.Domain;

namespace PostsModule.Application.GetPosts;

public class GetPostsCommandConsumer : IConsumer<GetPostsCommand>
{
    private readonly IPostsRepository repository;

    public GetPostsCommandConsumer(IPostsRepository repository)
    {
        this.repository = repository;
    }
    public async Task Consume(ConsumeContext<GetPostsCommand> context)
    {
        var posts = await repository.GetAll(context.Message.QueryModel);

        var mappedPosts = posts.Items.Select(post => new PostDto()
        {
            Id = post.Id.ToString(),
            Title = post.Title,
            FactionName = post.FactionName,
            CreatorId = post.CreatorId.ToString(),
            Description = post.Description,
            PrimaryColor = post.PrimaryColour,
            SecondaryColor = post.SecondaryColour,
            CreationDate = post.CreationDate,
            CreatorName = post.CreatorName,
            Images = post.Images
        }).ToList();

        var result = new GetPostsCommandResult()
        {
            Posts = mappedPosts,
            TotalCount = posts.TotalCount
        };

        
        await context.RespondAsync(CommandResult<GetPostsCommandResult>.Success(result));
    }
}
