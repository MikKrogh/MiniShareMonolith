namespace PostsModule.Domain;

public interface IImageStorageService
{
    public Task UploadImage(Stream stream, string directoryName, string fileName, string fileExtension);
    public Task<Stream> GetImage(string directoryName, string fileName);
}
