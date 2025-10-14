using PostsModule.Application.AddImage;
using PostsModule.Application.AddThumbnail;
using PostsModule.Application.Create;
using PostsModule.Application.DeletePost;
using PostsModule.Application.Get;
using PostsModule.Application.GetImage;
using PostsModule.Application.GetPosts;
using PostsModule.Domain;
using PostsModule.Infrastructure;
namespace PostsModule;

public static class ServiceExtensions
{
    public static void AddPostModuleServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<PostsContext>(options => options.EnableSensitiveDataLogging(false));
        serviceCollection.AddScoped<IPostsRepository, PostsRepository>();
        serviceCollection.AddTransient<IUserRepository, UserRepository>();
        serviceCollection.AddScoped<IImageRepository, ImageRepository>();
        serviceCollection.AddTransient<IDeletePostService, DeletePostService>();
        serviceCollection.AddScoped<IImageStorageService, AzureBlobService>();        
        serviceCollection.AddHostedService<DeletePostsProcessor>();
        serviceCollection.AddTransient<AddImageCommandConsumer>();
        serviceCollection.AddTransient<AddThumbnailCommandConsumer>();
        serviceCollection.AddTransient<CreatePostCommandConsumer>();
        serviceCollection.AddTransient<DeletionRequestedCommandConsumer>();
        serviceCollection.AddTransient<GetImageCommandConsumer>();
        serviceCollection.AddTransient<GetPostCommandConsumer>();
        serviceCollection.AddTransient<GetPostsCommandConsumer>();
    }
}
