using MassTransit;
namespace PostsModule;

public static class ServiceExtensions
{
	public static void AddPostsServiceExtensions(this IServiceCollection serviceCollection)
	{
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
