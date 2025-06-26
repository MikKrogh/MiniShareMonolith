

namespace PostsModule.Infrastructure;

internal class DeletePostJob
{
    public string PostId { get => Id; }
    public string Id { get; init; } = string.Empty;
    public bool IsCompleted  => 
        ImagesDeletionCompleted && 
        ThumbnailRemovedCompleted && 
        PostDeletedEventPublished;
    public bool ImagesDeletionCompleted { get; set; }
    public bool PostDataDeletionCompleted { get; set; }
    public bool ThumbnailRemovedCompleted { get; set; }
    public bool PostDeletedEventPublished { get; set; }

}