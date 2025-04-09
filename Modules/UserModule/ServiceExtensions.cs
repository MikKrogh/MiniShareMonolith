using Azure.Identity;

namespace UserModule;

public static class ServiceExtensions
{
    public static void AddUserModuleServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddTransient<IUserRepository, UserRepository>();
    }
    public static void UserModuleAppConfiguration(this IConfigurationBuilder configBuilder, ILogger logger)
    {
        var config = configBuilder.Build();
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production" || true)
        {
            configBuilder.AddAzureAppConfiguration(options =>
            {
                options.Connect(new Uri(config["AppConfigEndpoint"]), new DefaultAzureCredential())
                .Select("UserService*").TrimKeyPrefix("UserService:");
            });

            var confg = configBuilder.Build();
            var expectedTokens = confg["TableStorageAccount"];
            Console.WriteLine(expectedTokens);
            logger.LogError(expectedTokens);
            if (string.IsNullOrEmpty(expectedTokens))            
                throw new Exception("TableStorageAccount is not set in Azure App Configuration");



        }
    }
}
