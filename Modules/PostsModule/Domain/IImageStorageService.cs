namespace PostsModule.Domain;

public interface IImageStorageService
{
    public Task UploadImage(Stream stream, string directoryName, string fileName);
    public Task UploadThumbnail(Stream stream, string postId);
    public Task<Stream> GetImage(string directoryName, string fileName);

    public Task DeleteDirectory(string postId);
    public Task DeleteThumbnail(string postId);

    
}
