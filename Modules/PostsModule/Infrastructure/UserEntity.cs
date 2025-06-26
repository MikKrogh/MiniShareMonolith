

namespace PostsModule.Infrastructure;

internal class UserEntity
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public ICollection<PostEntity>? Posts { get; set; }
}
