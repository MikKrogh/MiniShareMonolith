using PostsModule.Domain;
namespace PostsModule.Application;

public sealed class PostDto
{
    public string Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; } = string.Empty;
    public string CreatorId { get; init; } = string.Empty;
    public string? CreatorName { get; init; } = string.Empty;
    public string FactionName { get; init; } = string.Empty;    

    public Colors PrimaryColor { get; init; }   

    public Colors SecondaryColor { get; init; }
    public IEnumerable<string> Images { get; init; } = new List<string>();
    public DateTime CreationDate { get; init; }
}
