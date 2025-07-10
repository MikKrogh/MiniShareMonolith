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
    private static bool _databaseInitialized;
    private static readonly object _lock = new();

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
            var sp = services.BuildServiceProvider();
            _postsContext = sp.GetRequiredService<PostsContext>();
            EnsureDatabaseCreated(_postsContext);

            services.AddSingleton<MesageBrokerFacade>(sp =>
            {
                return new MesageBrokerFacade(ibus, harness);
            });
        });
    }
    private static void EnsureDatabaseCreated(PostsContext context)
    {
        if (_databaseInitialized)
            return;

        lock (_lock)
        {
            if (_databaseInitialized)
                return;

            // Apply migrations or EnsureCreated
            context.Database.Migrate(); // preferred over EnsureCreated()
            _databaseInitialized = true;
        }
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
