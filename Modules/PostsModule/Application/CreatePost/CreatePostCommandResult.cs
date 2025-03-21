namespace PostsModule.Application.Create;

public sealed record CreatePostCommandResult
{
    public string PostId { get; init; } = string.Empty;
}
