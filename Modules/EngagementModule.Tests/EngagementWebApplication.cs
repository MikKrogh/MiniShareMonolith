
using BarebonesMessageBroker;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EngagementModule.Tests;

public class EngagementWebApplication : WebApplicationFactory<Program>
{
    public BarebonesMessageBroker.IBus MessageBroker;
    private EngagementDbContext _dbContext;
    private static bool _databaseInitialized;
    private static readonly object _lock = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {
            var sp = services.BuildServiceProvider();
            services.AddSingleton<IBus>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                return new TestBus(scopeFactory);
            });

            _dbContext = sp.GetRequiredService<EngagementDbContext>();
            MessageBroker = sp.GetRequiredService<IBus>();
        });
    }



    public async Task PublishPostCreatedEvent(string postId, string creatorId)
    {
        await MessageBroker.Publish(new
        {       
            PostId = postId,
            CreatorId = creatorId
        }, "PostModule.PostCreated");

    }
    private static void EnsureDatabaseCreated(EngagementDbContext context)
    {
        if (_databaseInitialized)
            return;
        lock (_lock)
        {
            if (_databaseInitialized)
                return;


            context.Database.Migrate();
            _databaseInitialized = true;
        }
    }
}
