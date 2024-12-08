using PostsModule.Domain;

namespace PostsModule.Application.Get;

public class GetPostResult
{
	public string Id { get; init; } = string.Empty;
	public string Title { get; private set; } = string.Empty;
	public string? Description { get; private set; }
	public string CreatorName { get; private set; } = string.Empty;
	public string CreatorId { get; private set; } = string.Empty;
	public Colours PrimaryColour { get; private set; } = Colours.Unknown;
	public Colours SecondaryColour { get; private set; } = Colours.Unknown;
	public DateTime CreationDate { get; init; }

}
