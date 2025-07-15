namespace PostsModule.Application.DeletePost;

public record PostDeletedEvent
{
    public PostDeletedEvent(string postId)
    {
        PostId = postId;
    }
    public string PostId { get; init; } = string.Empty;
}
