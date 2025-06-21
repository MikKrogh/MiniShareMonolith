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

public class PostsWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public MesageBrokerFacade MessageBrokerTestFacade { get; private set; }
    private PostsContext _postsContext;
    public void TruncateTables()
    {
        _postsContext.Database.ExecuteSqlRaw(@"DELETE FROM ""PostModule"".""Image"";");
        _postsContext.Database.ExecuteSqlRaw(@"DELETE FROM ""PostModule"".""Posts"";");
        _postsContext.Database.ExecuteSqlRaw(@"DELETE FROM ""PostModule"".""Users"";");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            services.AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumers(typeof(ServiceExtensions).Assembly);
                cfg.SetInMemorySagaRepositoryProvider();
                cfg.UsingInMemory((context, config) =>
                {
                    config.ConfigureEndpoints(context);
                });
            });

            var sp = services.BuildServiceProvider();
            _postsContext = sp.GetRequiredService<PostsContext>();

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
        MessageBrokerTestFacade = baseHost.Services.GetService<MesageBrokerFacade>();        
        return baseHost;
    }



    private async Task ThrowIfAzuriteNotRunning()
    {
        try
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://127.0.0.1:10000");
        }
        catch (Exception)
        {
            throw new Exception("Azurite is not running.");
        }
    }

    public async Task InitializeAsync()
    {
        await ThrowIfAzuriteNotRunning(); // azurite timeout when not running is too long, so we check it here
    }

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask; // exists because we want to use the initializeAsync from IAsyncLifetime
}
