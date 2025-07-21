
using BarebonesMessageBroker;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EngagementModule.Tests;

public class EngagementWebApplication : WebApplicationFactory<Program>
{
    public BarebonesMessageBroker.IBus MessageBroker;
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
}
