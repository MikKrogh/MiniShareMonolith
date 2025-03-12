namespace PostsModule.Application.AddImage;

public class AddImageCommand
{
    public Guid PostId { get; init; } =  Guid.Empty;
    public string FileExtension { get; init; } = string.Empty;
    public Guid StreamId { get; init; } = Guid.Empty;
}
