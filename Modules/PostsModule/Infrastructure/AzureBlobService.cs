using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using PostsModule.Domain;
using System.IO;

namespace PostsModule.Infrastructure;

public class AzureBlobService : IImageStorageService
{
    private const string containerName = "images";
    private readonly BlobContainerClient _blobClient;
    private bool blobExists = false;
    public AzureBlobService(IConfiguration config)
    {
        string? storgeAccount = config["BlobStorageUri"];
        if (string.IsNullOrEmpty(storgeAccount))
            throw new Exception("Cant connect to blob storage without a storage account uri");


        BlobServiceClient blobServiceClient;

        if (storgeAccount.Contains("UseDevelopmentStorage=true"))
        {
            blobServiceClient = new(storgeAccount);
        }
        else
        {
            blobServiceClient = new(new Uri("https://minisharestorageaccount.blob.core.windows.net/"), new DefaultAzureCredential());
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
        var path = Path.Combine(directoryName, fileName);
        var blobclient = _blobClient.GetBlobClient(path);
        await blobclient.UploadAsync(stream, overwrite:true);

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
        catch (Exception)
        {
            return null;
        }
    }

    public async Task UploadThumbnail(Stream stream, string postId)
    {
        await UploadImage(stream, "thumbnails", postId);
    }

    public async Task DeleteDirectory(string postId)
    {
        await foreach (BlobItem blobItem in _blobClient.GetBlobsAsync(prefix: postId))
        {
            BlobClient blob = _blobClient.GetBlobClient(blobItem.Name);
            await blob.DeleteIfExistsAsync();
        }


    }

    public async Task DeleteThumbnail(string postId)
    {
        var blobClient = _blobClient.GetBlobClient("thumbnails/"+ postId);
        await blobClient.DeleteAsync();
    }
}
