namespace PostsModule.Application.AddImage;

public sealed record AddImageCommand
{
    public Guid PostId { get; init; } = Guid.Empty;
    public string FileExtension { get; init; } = string.Empty;
    public Guid StreamId { get; init; } = Guid.Empty;
}
