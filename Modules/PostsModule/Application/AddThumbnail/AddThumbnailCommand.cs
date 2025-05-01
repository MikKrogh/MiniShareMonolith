namespace PostsModule.Application.AddThumbnail;

public sealed record AddThumbnailCommand
{
    public byte[] File { get; init; } = new byte[0];
    public string PostId { get; init; } = string.Empty;
}
