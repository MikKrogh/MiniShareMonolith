using Azure.Identity;
using UserModule.Features.CreateUser;
namespace UserModule;

public static class ServiceExtensions
{
    public static void AddUserModuleServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddTransient<IUserRepository, UserRepository>();
        serviceCollection.AddTransient<SignupCommandHandler>();
    }
    public static void UserModuleAppConfiguration(this IHostApplicationBuilder hostBuilder)
    {
        if (hostBuilder.Environment.IsProduction())
        {
            var config = hostBuilder.Configuration.Build();
            hostBuilder.Configuration.AddAzureAppConfiguration(options =>
            {
                options.Connect(new Uri(config["AppConfigEndpoint"]), new DefaultAzureCredential())
                .Select("UserService*").TrimKeyPrefix("UserService:");
            });
        }
    }
}
