using Azure.Identity;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using PostsModule.Domain;
using PostsModule.Domain.Auth;
using PostsModule.Infrastructure;
namespace PostsModule;

public static class ServiceExtensions
{
    public async static Task AddPostModuleServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<PostsContext>();
        serviceCollection.AddScoped<IPostsRepository, PostsRepository>();
        serviceCollection.AddScoped<IUserRepository, UserRepository>();
        serviceCollection.AddScoped<IImageRepository, ImageRepository>();
        serviceCollection.AddScoped<IImageStorageService, AzureBlobService>();
        serviceCollection.AddSingleton<IAuthHelper, JwtHandler>();

        //applies pending migrations to sql serer
        //var sc = serviceCollection.BuildServiceProvider();
        //var dbcontext = sc.GetRequiredService<PostsContext>();
        //await dbcontext.Database.MigrateAsync();
    }

    public static void AppConfiguration(this IConfigurationBuilder configBuilder)
    {
        var config = configBuilder.Build();
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            configBuilder.AddAzureAppConfiguration(options =>
            {
                options.Connect(new Uri(config["AppConfigEndpoint"]), new DefaultAzureCredential())
                .Select("PostService*").TrimKeyPrefix("PostService:")
                .Select("MiniShare_StorageAccount");
            });
        }
    }



}
