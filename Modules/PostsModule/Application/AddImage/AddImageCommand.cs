namespace PostsModule.Application.AddImage;

public class AddImageCommand
{
    //public FormFile Stream { get; init; }
    public string PostId { get; init; } = string.Empty;
    public string FileExtension { get; init; } = string.Empty;
    public Guid StreamId { get; init; }
}
