using MassTransit;
using PostsModule.Domain;
using PostsModule.Infrastructure;

namespace PostsModule.Application.DeletePost;

public class DeletionRequestedCommandConsumer : IConsumer<DeletionRequestedCommand>
{
    private readonly IPostsRepository postsRepository;
    private readonly IDeletePostService deletePostService;
    private readonly ILogger<DeletionRequestedCommandConsumer> logger;

    public DeletionRequestedCommandConsumer(IDeletePostService tmp, IPostsRepository postsRepository, IDeletePostService deletePostService, ILogger<DeletionRequestedCommandConsumer> logger)
    {
        this.tmp = tmp;
        this.postsRepository = postsRepository;
        this.deletePostService = deletePostService;
        this.logger = logger;
    }
    public async Task Consume(ConsumeContext<DeletionRequestedCommand> context)
    {
        var creatorId = await postsRepository.GetCreatorId(context.Message.PostId);
        if(creatorId != context.Message.UserId)
        {
            logger.LogWarning("User {UserId} attempted to delete post {PostId} but is not the creator.", context.Message.UserId, context.Message.PostId);
            await context.RespondAsync(CommandResult<DeletionRequestedCommandResult>.InternalError());
            return;
        }
        else
        {
            await deletePostService.CreateJob(context.Message.PostId);
            await context.RespondAsync(CommandResult<DeletionRequestedCommandResult>.Success(null));
        }

    }
}

public sealed record DeletionRequestedCommandResult;

