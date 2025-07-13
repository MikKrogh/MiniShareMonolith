

namespace PostsModule.Infrastructure;

internal class DeletePostJob
{
    public string Id { get; init; } = string.Empty;


    public bool ImagesDeletionCompleted { get; set; } = false;
    public bool PostDataDeletionCompleted { get; set; } = false; 
    public bool ThumbnailRemovedCompleted { get; set; } = false;
    public bool PostDeletedEventPublished { get; set; } = false; 
    public int FailedAttempts { get; set; } = 0; // rename to something other then failed, we increment on every attempt even if it succeeds

}