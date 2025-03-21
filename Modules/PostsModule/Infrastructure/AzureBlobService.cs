using Azure.Identity;
using Azure.Storage.Blobs;
using PostsModule.Domain;

namespace PostsModule.Infrastructure;

public class AzureBlobService : IImageStorageService
{
    private const string containerName = "images";
    private readonly BlobContainerClient _blobClient;
    private bool blobExists = false;
    public AzureBlobService(IConfiguration config)
    {
        string storgeAccount = config["StorageAccountUri"];
        if (string.IsNullOrEmpty(storgeAccount))
            throw new Exception("Cant connect to blob storage without a storage account uri");


        BlobServiceClient blobServiceClient;

        if (storgeAccount == "UseDevelopmentStorage=true")
        {
            blobServiceClient = new(storgeAccount);
        }
        else
        {
            blobServiceClient = new(new Uri(storgeAccount), new DefaultAzureCredential());
        }

        _blobClient = blobServiceClient.GetBlobContainerClient(containerName);
    }
    public async Task UploadImage(Stream stream, string directoryName, string fileName)
    {
        if (!blobExists)
        {
            await _blobClient.CreateIfNotExistsAsync();
            blobExists = true;
        }
        try
        {
            var path = Path.Combine(directoryName, fileName);            
            await _blobClient.UploadBlobAsync(path, stream);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<Stream> GetImage(string directoryName, string fileName)
    {
        try
        {
            var path = Path.Combine(directoryName, fileName);
            var blobClient = _blobClient.GetBlobClient(path);
            var response = await blobClient.OpenReadAsync();
            return response;
        }
        catch (Exception e)
        {
            return null;
        }
    }
}
