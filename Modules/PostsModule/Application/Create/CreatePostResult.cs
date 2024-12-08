namespace PostsModule.Application.Create;

public record CreatePostResult
{
	public string PostId { get; init; } = string.Empty;
}