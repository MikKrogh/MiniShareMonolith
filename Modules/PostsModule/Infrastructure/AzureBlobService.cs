
using PostsModule.Domain;
namespace PostsModule.Infrastructure;

public class AzureBlobService : IImageStorageService
{
    private const string containerName = "images";
    private bool blobExists = false;
    public AzureBlobService(IConfiguration config)
    {



    }
    public async Task UploadImage(Stream stream, string directoryName, string fileName)
    {

    }

    public async Task<Stream> GetImage(string directoryName, string fileName)
    {
        return null;
    }

    public async Task UploadThumbnail(Stream stream, string postId)
    {
        await UploadImage(stream, "thumbnails", postId);
    }

    public async Task DeleteDirectory(string postId)
    {

    }

    public async Task DeleteThumbnail(string postId)
    {

    }
}
