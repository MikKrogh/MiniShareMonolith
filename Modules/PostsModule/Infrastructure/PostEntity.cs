using System.ComponentModel.DataAnnotations.Schema;

namespace PostsModule.Infrastructure;
internal record PostEntity
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public string CreatorId { get; set; } = string.Empty;
    public UserEntity? Creator { get; set; }
    public string Faction { get; set; } = string.Empty;

    public string PrimaryColour { get; set; } = string.Empty;
    public string? SecondaryColour { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; } = DateTime.Now;
}
internal class UserEntity
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public ICollection<PostEntity>? Posts { get; set; }
}