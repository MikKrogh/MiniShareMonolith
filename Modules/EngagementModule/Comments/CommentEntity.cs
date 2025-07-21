namespace EngagementModule.Comments;

internal class CommentEntity
{
    public string CommentId { get; init; } = Guid.NewGuid().ToString();
    public string? ParentCommentId { get; init; } = string.Empty; 
    public string PostId { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
