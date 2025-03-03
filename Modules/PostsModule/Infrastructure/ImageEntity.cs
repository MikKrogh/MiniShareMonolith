namespace PostsModule.Infrastructure;

internal record ImageEntity
{
    public string Id { get; init; } = string.Empty;
    public string PostId { get; init; } = string.Empty;
    public PostEntity Post { get; init; } = new();
}
