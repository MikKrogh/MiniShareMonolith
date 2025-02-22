using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PostsModule.Domain;
using PostsModule.Infrastructure;
using PostsModule.Tests.Helper;

namespace PostsModule.Tests;

public class PostsWebApplicationFactory: WebApplicationFactory<Program>
{
    public FakeImageBlobStorage FakeImageBlobStorage { get; private set; }
    public MessageBrokerTestFacade MessageBrokerTestFacade { get; private set; }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
 
        builder.ConfigureServices(services =>
        {
            ConfigueTestDependencies(services);
            InitialDatabaseSetup(services);


            services.AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumers(typeof(ServiceExtensions).Assembly);
                cfg.SetInMemorySagaRepositoryProvider();
                cfg.UsingInMemory((context, config) =>
                {
                    config.ConfigureEndpoints(context);
                });                
            });
            services.AddSingleton<MessageBrokerTestFacade>(sp =>
            {
                var ibus = sp.GetRequiredService<IBus>();
                var harness = sp.GetRequiredService<ITestHarness>();
                return new MessageBrokerTestFacade(ibus, harness);
            });

        });
    }


    private void ConfigueTestDependencies(IServiceCollection services)
    {
        FakeImageBlobStorage = new();
        services.AddSingleton<IImageStorageService>(FakeImageBlobStorage);
    }
    private void InitialDatabaseSetup(IServiceCollection services)
    {
        InitialEntityFrameWorkSetup(services);
    }
    private void InitialEntityFrameWorkSetup(IServiceCollection services)
    {
        var sp = services.BuildServiceProvider();

        using (var scope = sp.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<PostsContext>();
            db.Database.Migrate();
        }
    }
}