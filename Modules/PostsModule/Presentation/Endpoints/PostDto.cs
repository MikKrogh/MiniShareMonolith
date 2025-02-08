using PostsModule.Domain;

namespace PostsModule.Presentation.Endpoints;

public record PostDto
{
	public string Id { get; init; }
	public string Title { get; init; } = string.Empty;
	public string? Description { get; init; } = string.Empty;
	public string CreatorId { get; init; } = string.Empty ;
	public string? CreatorName { get; init; } = string.Empty ;
	public string FactionName { get; init; } = string.Empty;
    public Colours PrimaryColor { get; init; } 
	public Colours SecondaryColor { get; init; }
	public IFormFileCollection? Images { get; init; }
}
