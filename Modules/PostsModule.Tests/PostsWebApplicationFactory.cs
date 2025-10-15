using BarebonesMessageBroker;
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
    private static bool _databaseInitialized;
    private static readonly object _lock = new();

    public MesageBrokerFacade MessageBrokerTestFacade { get; private set; }
    private PostsContext _postsContext;
    public void TruncateTables()
    {
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
            services.AddSingleton<MesageBrokerFacade>();
            services.AddTransient<IPresignedUrlGenerator, PresignedUrlGeneratorMock>();
            services.AddSingleton<BarebonesMessageBroker.IBus>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                return new TestBus(scopeFactory);
            });
        });
    }
    private static void EnsureDatabaseCreated(PostsContext context)
    {
        if (_databaseInitialized)
            return;

        lock (_lock)
        {
            context.Database.Migrate(); 
            _databaseInitialized = true;
        }
    }
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var baseHost = base.CreateHost(builder);
        MessageBrokerTestFacade = baseHost.Services.GetService<MesageBrokerFacade>();        
        return baseHost;
    }


}
