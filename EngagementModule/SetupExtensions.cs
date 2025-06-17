namespace EngagementModule;

public static class SetupExtensions
{
    public static void AddEngagementModuleServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<EngagementDbContext>(options => options.EnableSensitiveDataLogging(false));
        serviceCollection.AddTransient<IPostLikeService, EngagementDbContext>();
    }
}
