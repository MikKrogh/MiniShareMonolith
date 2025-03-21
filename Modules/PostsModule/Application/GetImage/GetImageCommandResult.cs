namespace PostsModule.Application.GetImage;

public sealed record GetImageCommandResult
{
    public byte[] File { get; init; } = new byte[0];
}
