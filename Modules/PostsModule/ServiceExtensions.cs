using Azure.Identity;
using MassTransit;
using PostsModule.Domain;
using PostsModule.Domain.Auth;
using PostsModule.Infrastructure;
namespace PostsModule;

public static class ServiceExtensions
{
    public static void AddPostModuleServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<PostsContext>();
        serviceCollection.AddScoped<IPostsRepository, PostsRepository>();
        serviceCollection.AddScoped<IUserRepository, UserRepository>();
        serviceCollection.AddScoped<IImageRepository, ImageRepository>();
        serviceCollection.AddScoped<IImageStorageService, AzureBlobService>();
        serviceCollection.AddSingleton<IAuthHelper, JwtHandler>();


        serviceCollection.AddMassTransit(x =>
        {
            x.AddConsumers(typeof(ServiceExtensions).Assembly);
            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });
    }

    public static void AppConfiguration(this IConfigurationBuilder configBuilder)
    {
        var config = configBuilder.Build();
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            configBuilder.AddAzureAppConfiguration(options =>
            {
                options.Connect(new Uri(config.GetConnectionString("AppConfigEndpoint")), new DefaultAzureCredential())
                .Select("PostService*").TrimKeyPrefix("PostService:");
            });
        }
    }



}
