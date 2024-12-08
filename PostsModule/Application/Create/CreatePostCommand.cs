namespace PostsModule.Application.Create;

public record CreatePostCommand
{
	public string Title { get; init; } = "";
	public string? Description { get; init; }
	public string CreatorId { get; init; } = "";
	public string PrimaryColor { get; init; } = "";
	public string SecondaryColor { get; init; } = "";

}