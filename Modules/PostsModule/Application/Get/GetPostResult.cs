using PostsModule.Domain;

namespace PostsModule.Application.Get;

public class GetPostResult
{
    public string Id { get; init; } = string.Empty;
	public string Title { get; init; } = string.Empty;
	public string? Description { get; init; }
	public string CreatorName { get; init; } = string.Empty;
	public string CreatorId { get; init; } = string.Empty;
	public Colours PrimaryColour { get; init; } = Colours.Unknown;
	public Colours SecondaryColour { get; init; } = Colours.Unknown;
	public DateTime CreationDate { get; init; }
}
