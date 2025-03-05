using MassTransit;
using PostsModule.Domain;
using PostsModule.Domain.Auth;
using PostsModule.Infrastructure;
namespace PostsModule;

public static class ServiceExtensions
{
	public static void AddPostsServiceExtensions(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
        serviceCollection.AddDbContext<PostsContext>();
		serviceCollection.AddScoped<IPostsRepository, PostsRepository>();
		serviceCollection.AddScoped<IUserRepository, UserRepository>();
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

}
