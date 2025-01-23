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

        var response = new GetPostResult() 
        {
            Id = post.Id.ToString(),
            Title = post.Title,
            Description = post.Description,
            CreatorName = post.CreatorName,
            CreatorId = post.CreatorId,
            PrimaryColour = post.PrimaryColour,
            SecondaryColour = post.SecondaryColour,
            CreationDate = post.CreationDate
        };
		await context.RespondAsync(response);
    }
}
