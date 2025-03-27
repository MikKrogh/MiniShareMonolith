using MassTransit.Testing;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using UserModule;
using EventMessages;

namespace UserModule.Tests;

public class UserWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {
            services.AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<MyEventConsumer>();

                cfg.AddConsumers(typeof(ServiceExtensions).Assembly);
                cfg.SetInMemorySagaRepositoryProvider();
                cfg.UsingInMemory((context, config) =>
                {
                    config.ConfigureEndpoints(context);
                });
            });
        });
    }
}
public class MyEventConsumer : IConsumer<UserCreatedEvent>
{
    public Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        Console.WriteLine($"Received event: {context.Message.UserName}");
        return Task.CompletedTask;
    }
}
