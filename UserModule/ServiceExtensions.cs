using Azure.Identity;

namespace UserModule;

public static class ServiceExtensions
{
    public static void AddUserModuleServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddTransient<IUserRepository, UserRepository>();
    }
    public static void UserModuleAppConfiguration(this IConfigurationBuilder configBuilder)
    {
        var config = configBuilder.Build();
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            configBuilder.AddAzureAppConfiguration(options =>
            {
                options.Connect(new Uri(config["AppConfigEndpoint"]), new DefaultAzureCredential())
                .Select("UserService*").TrimKeyPrefix("UserService:");                
            });
        }
    }
}
