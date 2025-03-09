namespace PostsModule.Domain;

public class Image
{
    public string FileName { get; private set; } = string.Empty;
    public string PostId { get; private set; } = string.Empty;
    private Image()
    {
    }

    public static Image Create(string fileName, string postId)
    {
        if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("image cannot have empty fileName");
        if (string.IsNullOrEmpty(postId)) throw new ArgumentException("image cannot have empty postId");
        return new Image() { FileName = fileName, PostId = postId };
    }
}
