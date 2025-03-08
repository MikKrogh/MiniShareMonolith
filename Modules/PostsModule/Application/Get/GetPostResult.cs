using PostsModule.Domain;

namespace PostsModule.Application.Get;

public class GetPostResult
{
    public string Id { get; init; } = string.Empty;
	public string Title { get; init; } = string.Empty;
	public string Faction { get; init; } = string.Empty;
	public string? Description { get; init; }
	public string CreatorName { get; init; } = string.Empty;
	public string CreatorId { get; init; } = string.Empty;
	public IReadOnlyCollection<string> Images { get; init; } = new List<string>();
	public Colors PrimaryColor { get; init; } = Colors.Unknown;
	public Colors SecondaryColor { get; init; } = Colors.Unknown;
	public DateTime CreationDate { get; init; }
}
