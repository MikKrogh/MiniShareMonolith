using Azure.Identity;
using PostsModule.Application.DeletePost;
using PostsModule.Domain;
using PostsModule.Domain.Auth;
using PostsModule.Infrastructure;
namespace PostsModule;

public static class ServiceExtensions
{
    public static void AddPostModuleServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<PostsContext>(options => options.EnableSensitiveDataLogging(false));
        serviceCollection.AddScoped<IPostsRepository, PostsRepository>();
        serviceCollection.AddScoped<IUserRepository, UserRepository>();
        serviceCollection.AddScoped<IImageRepository, ImageRepository>();
        serviceCollection.AddTransient<IDeletePostService, DeletePostService>();
        serviceCollection.AddScoped<IImageStorageService, AzureBlobService>();
        serviceCollection.AddSingleton<IAuthHelper, JwtHandler>();
        serviceCollection.AddHostedService<DeletePostsProcessor>();
    }

    public static void PostModuleAppConfiguration(this IHostApplicationBuilder hostBuilder)
    {
        var config = hostBuilder.Configuration.Build();
        if (hostBuilder.Environment.IsProduction())
        {
            hostBuilder.Configuration.AddAzureAppConfiguration(options =>
            {
                options.Connect(new Uri(config["AppConfigEndpoint"]), new DefaultAzureCredential())
                .Select("PostService*").TrimKeyPrefix("PostService:");
            });
        }
    }
}
