namespace PostsModule.Infrastructure;
internal record PostEntity
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public string CreatorId { get; set; } = string.Empty;
    public UserEntity? Creator { get; set; }
    public string Faction { get; set; } = string.Empty;

    public string PrimaryColor { get; set; } = string.Empty;
    public string? SecondaryColor { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; } = DateTime.Now;
}
