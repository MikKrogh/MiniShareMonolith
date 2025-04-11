using Azure.Identity;
namespace UserModule;

public static class ServiceExtensions
{
    public static void AddUserModuleServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddTransient<IUserRepository, UserRepository>();
    }
    public static void UserModuleAppConfiguration(this IHostApplicationBuilder hostBuilder)
    {
        var config = hostBuilder.Configuration.Build();
        if (hostBuilder.Environment.IsProduction())
        {
            hostBuilder.Configuration.AddAzureAppConfiguration(options =>
            {
                options.Connect(new Uri(config["AppConfigEndpoint"]), new DefaultAzureCredential())
                .Select("UserService*").TrimKeyPrefix("UserService:");
            });
        }
    }
}
