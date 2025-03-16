using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PostsModule.Infrastructure;
using PostsModule.Tests.Helper;

namespace PostsModule.Tests;

public class PostsWebApplicationFactory : WebApplicationFactory<Program>
{
    public MesageBrokerFacade MessageBrokerTestFacade { get; private set; }
    private PostsContext _postsContext;
    public  void TruncateTables()
    {
        _postsContext.Database.ExecuteSqlRaw("DELETE FROM PostModule.Image;");
        _postsContext.Database.ExecuteSqlRaw("DELETE FROM PostModule.Posts;");
        _postsContext.Database.ExecuteSqlRaw("DELETE FROM PostModule.Users;");
    }

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
            services.AddSingleton<MesageBrokerFacade>(sp =>
            {
                var ibus = sp.GetRequiredService<IBus>();
                var harness = sp.GetRequiredService<ITestHarness>();
                return new MesageBrokerFacade(ibus, harness);
            });
        });


    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var baseHost = base.CreateHost(builder);
        var sp = baseHost.Services.GetService<MesageBrokerFacade>();
        MessageBrokerTestFacade = sp;
        return baseHost;
    }


    private void ConfigueTestDependencies(IServiceCollection services)
    {
    }
    private void InitialDatabaseSetup(IServiceCollection services)
    {
        InitialEntityFrameWorkSetup(services);
    }
    private void InitialEntityFrameWorkSetup(IServiceCollection services)
    {
        var sp = services.BuildServiceProvider();
        _postsContext = sp.GetRequiredService<PostsContext>();

        _postsContext.Database.Migrate();
        TruncateTables();
    }
}