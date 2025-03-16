namespace PostsModule.Domain;

public interface IImageStorageService
{
    public Task UploadImage(Stream stream, string directoryName, string fileName);
    public Task<Stream> GetImage(string directoryName, string fileName);
}
