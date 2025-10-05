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

internal class ActivityChain
{
    public string Id { get; set; } = string.Empty;
    public string PostId { get; set; } = string.Empty;
    public DateTime DateChanged { get; set; }
    public List<ChainLink> Chains { get; set; } = new List<ChainLink>();

}

internal class ChainLink
{
    public string UserId { get; set; } = string.Empty;
    public string AcitivtyChainId { get; set; } = string.Empty;
    public ActivityChain Chain { get; set; } = new ActivityChain();
}

internal class UserLastSync
{
    public string UserId { get; set; } = string.Empty;
    public DateTime LastSyncTime { get; set; } = DateTime.UtcNow;    
}