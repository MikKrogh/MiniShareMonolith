using MassTransit;

namespace UserSignupModule;

public static class ServiceExtensions
{
    public static void AddUserSignupServiceExtensions(this IServiceCollection serviceCollection, IConfiguration configuration)
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
