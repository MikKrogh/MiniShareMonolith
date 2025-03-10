using Azure.Identity;
using Azure.Storage.Blobs;
using PostsModule.Domain;
using System.Runtime.CompilerServices;

namespace PostsModule.Infrastructure;

public class AzureBlobService : IImageStorageService
{
    private const string containerName = "images";
    private readonly BlobContainerClient _blobClient;
    public AzureBlobService(IConfiguration config)
    {
        string storgeAccount = config["StorageAccountUri"];
        if (string.IsNullOrEmpty(storgeAccount))
            throw new Exception("Cant connect to blob storage without a storage account uri");


        var blobServiceClient = new BlobServiceClient(
            new Uri(storgeAccount),
            new DefaultAzureCredential()
        );
        _blobClient = blobServiceClient.GetBlobContainerClient(containerName);
    }
    public async Task UploadImage(Stream stream, string directoryName, string fileName, string fileExtension)
    {
        try
        {
            var path = Path.Combine(directoryName, fileName + fileExtension);            
            await _blobClient.UploadBlobAsync(path, stream);
        }
        catch (Exception e)
        {
            throw;
        }

    }


}
