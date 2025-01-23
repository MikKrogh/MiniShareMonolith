using MassTransit;
using PostsModule.Domain;
namespace PostsModule.Application.Create;

public class CreatePostConsumer : IConsumer<CreatePostCommand>
{
    private readonly IPostsRepository repository;

    public CreatePostConsumer(IPostsRepository repository)
    {
        this.repository = repository;
    }
    public async Task Consume(ConsumeContext<CreatePostCommand> context)
	{
        var post = Post.CreateNew(context.Message.Title, context.Message.CreatorId, context.Message.FactionName);
        post.SetDescription(context.Message.Description);
        post.SetPrimaryColour(context.Message.PrimaryColor);
        post.SetSecondaryColour(context.Message.SecondaryColor);
        
        await repository.Save(post);

        var response = new CreatePostResult 
		{
			PostId = post.Id.ToString(),
        };
		await context.RespondAsync(response);
	}
}
