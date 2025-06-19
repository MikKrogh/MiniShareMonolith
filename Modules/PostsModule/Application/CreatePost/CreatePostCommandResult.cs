namespace PostsModule.Application.Create;

public  record CreatePostCommandResult
{
    public string PostId { get; init; } = string.Empty;
}
