namespace PostsModule.Domain;

public interface IImageStorageService
{
    public Task<IResult> UploadImage(Stream stream, string directoryName, string fileName);
}
