
using PostsModule.Application.Create;
using PostsModule.Application.DeletePost;
using PostsModule.Application.Get;
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
        serviceCollection.AddTransient<IPresignedUrlGenerator, CloudflarePresign>();
        
        serviceCollection.AddTransient<IDeletePostService, DeletePostService>(); 
        serviceCollection.AddHostedService<DeletePostsProcessor>();
        serviceCollection.AddTransient<CreatePostCommandConsumer>();
        serviceCollection.AddTransient<DeletionRequestedCommandConsumer>();
        serviceCollection.AddTransient<GetPostCommandConsumer>();
        serviceCollection.AddTransient<GetPostsCommandConsumer>();
    }
}
