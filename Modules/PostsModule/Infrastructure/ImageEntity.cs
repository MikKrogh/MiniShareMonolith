namespace PostsModule.Infrastructure;

internal class ImageEntity
{
    public string Id { get; set; }
    public string PostId { get; set; }
    public PostEntity? Post { get; set; } 
}
