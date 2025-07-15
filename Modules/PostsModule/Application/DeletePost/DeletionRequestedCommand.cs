namespace PostsModule.Application.DeletePost;

public sealed record DeletionRequestedCommand 
{
    public string PostId { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
}
