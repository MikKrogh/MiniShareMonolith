
using UserModule.Features.CreateUser;
using UserModule.Features.GetUser;
using UserModule.Features.GetUsers;
namespace UserModule;

public static class ServiceExtensions
{
    public static void AddUserModuleServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddTransient<IUserRepository, UserRepository>();
        serviceCollection.AddTransient<SignupCommandHandler>();
        serviceCollection.AddTransient<GetUserCommandHandler>();
        serviceCollection.AddTransient<GetUsersCommandHandler>();
    }
}
