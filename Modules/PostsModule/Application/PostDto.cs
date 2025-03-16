using PostsModule.Domain;
using System.Text.Json.Serialization;

namespace PostsModule.Application;

public class PostDto
{
    public string Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; } = string.Empty;
    public string CreatorId { get; init; } = string.Empty;
    public string? CreatorName { get; init; } = string.Empty;
    public string FactionName { get; init; } = string.Empty;
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public Colors PrimaryColor { get; init; }
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public Colors SecondaryColor { get; init; }
    public IEnumerable<string> Images { get; init; } = new List<string>();
    public DateTime CreationDate { get; init; }
}
