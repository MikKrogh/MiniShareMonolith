
using BarebonesMessageBroker;
using Microsoft.EntityFrameworkCore;
using PostsModule.Application.DeletePost;

namespace PostsModule.Infrastructure;

internal sealed class DeletePostService:  IDeletePostService
{
    private readonly PostsContext _context;
    
    private readonly ILogger<DeletePostService> logger;
    private readonly IBus bus;

    public DeletePostService(PostsContext context, ILogger<DeletePostService> logger, IBus bus)
    {
        _context = context;
        this.logger = logger;
        this.bus = bus;
    }
    
    public async Task TryDelete(string postId)
    {
        logger.LogInformation("started deletion process for post: {0}", postId);
        var entity = await _context.DeletionJobs.FirstOrDefaultAsync( x => x.Id == postId);
        if (entity == null) return;
        entity.FailedAttempts++;
        if (!entity.PostDataDeletionCompleted)
        {
            var success = await DeletePostRelation(postId);
            if (success)            
                entity.PostDataDeletionCompleted = true;
        }
        if (!entity.ImagesDeletionCompleted)
        {
            var success = await DeleteImageRelations(postId);
            if (success) 
                entity.ImagesDeletionCompleted = true;            
        }
        if (!entity.ThumbnailRemovedCompleted)
        {
            var success = await DeleteThumbnail(postId);    
            if (success) 
                entity.ThumbnailRemovedCompleted = true;
        }
        if (!entity.PostDeletedEventPublished)
        {
            var success = await PublishPostDeletedEvent(postId);
            if (success) 
                entity.PostDeletedEventPublished = true;
        }

        _context.DeletionJobs.Update(entity);
        await _context.SaveChangesAsync();
        logger.LogInformation("finished deletion process for post: {0}", postId);
    }
    public async Task<IEnumerable<string>> FetchUnfinishedJobs(int take = 20 )
    {
        var incompleteJobs = _context.DeletionJobs
            .Where(job =>
                (!job.ImagesDeletionCompleted ||
                !job.PostDataDeletionCompleted ||
                !job.ThumbnailRemovedCompleted ||
                !job.PostDeletedEventPublished) && 
                job.FailedAttempts < 4) 
            .Select(x => x.Id)
            .Take(take);
            

        return await incompleteJobs.ToListAsync();
    }

    public async Task CreateJob(string postId)
    {
        var entity = new DeletePostJob()
        {
            Id = postId,
        };
        _context.DeletionJobs.Add( entity );
        await _context.SaveChangesAsync();
    }
    private async Task<bool> DeleteImageRelations(string postId)
    {
        throw new NotImplementedException("Image deletion not implemented yet");

    }
    private async Task<bool> DeleteThumbnail(string postId)
    {
        throw new NotImplementedException("Image deletion not implemented yet");
    }

    private async Task<bool> PublishPostDeletedEvent(string postId)
    {
        try
        {
            await bus.Publish(new PostDeletedEvent(postId), "PostModule.PostDeleted");
            return true;            
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish post deleted event for post {0}", postId);
            return false;
        }
    }

    private async Task<bool> DeletePostRelation(string postId)
    {
        try
        {
            var entity = await _context.Posts.FirstOrDefaultAsync(x => x.Id == postId);
            if(entity != null)
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete post relation for post {0}", postId);
            return false;
        }

    }

    
}
public interface IDeletePostService
{
    Task TryDelete(string postId);
    Task<IEnumerable<string>> FetchUnfinishedJobs(int take);
    Task CreateJob(string postId);
}