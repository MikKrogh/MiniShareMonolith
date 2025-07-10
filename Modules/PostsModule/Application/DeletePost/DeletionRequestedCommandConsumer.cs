using PostsModule.Domain;
using PostsModule.Infrastructure;

namespace PostsModule.Application.DeletePost;

public class DeletionRequestedCommandConsumer
{
    private readonly IPostsRepository postsRepository;
    private readonly IDeletePostService deletePostService;
    private readonly ILogger<DeletionRequestedCommandConsumer> logger;

    public DeletionRequestedCommandConsumer( IPostsRepository postsRepository, IDeletePostService deletePostService, ILogger<DeletionRequestedCommandConsumer> logger)
    {
        this.postsRepository = postsRepository;
        this.deletePostService = deletePostService;
        this.logger = logger;
    }
    public async Task<CommandResult<DeletionRequestedCommandResult>> Consume(DeletionRequestedCommand context)
    {
        var creatorId = await postsRepository.GetCreatorId(context.PostId);
        if(creatorId != context.UserId)
        {
            logger.LogWarning("User {UserId} attempted to delete post {PostId} but is not the creator.", context.UserId, context.PostId);
            return CommandResult<DeletionRequestedCommandResult>.InternalError();            
        }
        else
        {
            await deletePostService.CreateJob(context.PostId);
            return  CommandResult<DeletionRequestedCommandResult>.Success(null);
        }

    }
}

public sealed record DeletionRequestedCommandResult;

