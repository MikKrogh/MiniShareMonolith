using Azure.Identity;
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
        serviceCollection.AddScoped<IImageStorageService, AzureBlobService>();
        serviceCollection.AddSingleton<IAuthHelper, JwtHandler>();
    }

    public static void PostModuleAppConfiguration(this IConfigurationBuilder configBuilder)
    {
        var config = configBuilder.Build();
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            configBuilder.AddAzureAppConfiguration(options =>
            {
                options.Connect(new Uri(config["AppConfigEndpoint"]), new DefaultAzureCredential())
                .Select("PostService*").TrimKeyPrefix("PostService:");
            });
        }
    }
}
