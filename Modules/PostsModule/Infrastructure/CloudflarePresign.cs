using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
namespace PostsModule.Infrastructure;

public interface IPresignedUrlGenerator
{
    public Task<IEnumerable<string>> GetPresignedUris(string dirName, int count);
    public Task<string> GetPresignedThumbnailUrl(string postId);

}

public class CloudflarePresign  : IPresignedUrlGenerator
{
    private readonly string _accesKEey;
    private readonly string _secretAccessKey;
    private readonly string _serviceUrl;
    private const string _bucketName = "mini-share";
    private readonly ILogger<CloudflarePresign> logger;

    public CloudflarePresign(IConfiguration config, ILogger<CloudflarePresign> logger)
    {
        this.logger = logger;
        
        _accesKEey              = config["PostModule.PreSignUri.accessKey"];
        _secretAccessKey        = config["PostModule.PreSignUri.secretAccessKey"];
        _serviceUrl             = config["PostModule.PreSignUri.S3Endpoint"];


        if (string.IsNullOrEmpty(_serviceUrl)) throw new InvalidOperationException("R2 Key is not set.");
        if (string.IsNullOrEmpty(_accesKEey)) throw new InvalidOperationException("Access Key is not set.");
        if (string.IsNullOrEmpty(_secretAccessKey)) throw new InvalidOperationException("Secret Key is not set.");        
    }


    public async Task<IEnumerable<string>> GetPresignedUris(string dirName, int count = 8)
    {
        if (string.IsNullOrEmpty(dirName) || count < 1) 
            throw new ArgumentException("Invalid arguments provided.");
        
        var credentials = new BasicAWSCredentials(_accesKEey, _secretAccessKey);
        IAmazonS3 s3Client = new AmazonS3Client(credentials, new AmazonS3Config
        {
            ServiceURL = _serviceUrl,
        });

        string[] signedUrls = new string[count];
        for (int i = 0; i < count; i++)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = GenerateFileKey(dirName,count),
                Verb = HttpVerb.PUT,
                Expires = DateTime.Now.AddMinutes(8),
            };
            signedUrls[i] = await FetchPresignedUrl(s3Client, request);
        }
        return signedUrls;
    }

    public async Task<string> GetPresignedThumbnailUrl(string postId)
    {
        if (string.IsNullOrEmpty(postId))
            throw new ArgumentException("Invalid arguments provided.");

        var credentials = new BasicAWSCredentials(_accesKEey, _secretAccessKey);
        IAmazonS3 s3Client = new AmazonS3Client(credentials, new AmazonS3Config
        {
            ServiceURL = _serviceUrl,
        });

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = Path.Combine("thumbnails", postId),
            Verb = HttpVerb.PUT,
            Expires = DateTime.Now.AddMinutes(8),
        };
        return await FetchPresignedUrl(s3Client, request);        
    }
    
    private async Task<string> FetchPresignedUrl(IAmazonS3 s3Client, GetPreSignedUrlRequest request)
    {
        try
        {
            var signedUrl =  await s3Client.GetPreSignedURLAsync(request);
            if (string.IsNullOrEmpty(signedUrl))
            {
                await Task.Delay(500); // brief delay before retry
                signedUrl = await FetchPresignedUrl(s3Client, request);

                if (string.IsNullOrEmpty(signedUrl))
                    throw new Exception("Failed to generate presigned URL after retry.");
            }            
            return signedUrl;
        }
        catch (Exception e)
        {
            string postId = request.Key.Split('/')[1];
            string logmessage = $"Error generating presigned URL for post {postId}: {e.Message}";
            logger.LogError(logmessage);
            throw;
        }
    }

    private string GenerateFileKey(string directive, int fileName) => $"{directive}/{fileName}";
}
