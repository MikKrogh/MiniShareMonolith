using MassTransit;
using Microsoft.EntityFrameworkCore;
using PostsModule.Domain;
using System.Threading;

namespace PostsModule.Infrastructure;

internal sealed class DeletePostService:  IDeletePostService
{
    private readonly PostsContext _context;
    private readonly IImageStorageService blobStorage;
    private readonly ILogger<DeletePostService> logger;

    public DeletePostService(PostsContext context, IImageStorageService blobStorage, ILogger<DeletePostService> logger)
    {
        _context = context;
        this.blobStorage = blobStorage;
        this.logger = logger;
    }
    
    public async Task TryDelete(string postId)
    {
        logger.LogInformation("started deletion process for post: {0}", postId);
        var entity = await _context.DeletionJobs.FirstOrDefaultAsync( x => x.Id == postId);
        if (entity == null) return;        

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

        _context.DeletionJobs.Update(entity);
        await _context.SaveChangesAsync();
        logger.LogInformation("finished deletion process for post: {0}", postId);
    }
    public async Task<IEnumerable<string>> FetchUnfinishedJobs(int take = 20 )
    {
        var incompleteJobs = _context.DeletionJobs
            .Where(job =>
                !job.ImagesDeletionCompleted ||
                !job.PostDataDeletionCompleted ||
                !job.ThumbnailRemovedCompleted ||
                !job.PostDeletedEventPublished)
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
        try
        {
            await blobStorage.DeleteDirectory(postId);            
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete images in blob for post {0}", postId);
            return false;
        }
    }
    private async Task<bool> DeleteThumbnail(string postId)
    {
        try
        {
            await blobStorage.DeleteThumbnail(postId);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete thumbnail in blob for post {0}", postId);
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