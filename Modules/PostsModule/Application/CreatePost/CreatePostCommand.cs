namespace PostsModule.Application.Create;

public  record CreatePostCommand
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string CreatorId { get; init; } = string.Empty;
    public string FactionName { get; init; } = string.Empty;
    public string PrimaryColor { get; init; } = string.Empty;
    public string SecondaryColor { get; init; } = string.Empty;
}