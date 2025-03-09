using PostsModule.Domain;

namespace PostsModule.Infrastructure;

public class AzureBlobService : IImageStorageService
{
    public AzureBlobService(IConfiguration config)
    {
        string connectionString = config["BlobConnectionString"];
        Console.WriteLine();
    }
    public Task<IResult> UploadImage(Stream stream, string directoryName, string fileName)
    {
        throw new NotImplementedException();
    }
}
