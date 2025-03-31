using MassTransit;
using UserModule.Features.GetUser;
using UserModule.Features.ManuelUserSignup;

namespace UserModule;

public static class ServiceExtensions
{
    public static void AddUserModuleServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddMassTransit(x =>
        {
            x.AddConsumer<SignupCommandHandler>();
            x.AddConsumer<GetUserCommandHandler>();            
            x.AddConsumers(typeof(ServiceExtensions).Assembly);
            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });
        serviceCollection.AddTransient<IUserRepository, UserRepository>();
    }

}
