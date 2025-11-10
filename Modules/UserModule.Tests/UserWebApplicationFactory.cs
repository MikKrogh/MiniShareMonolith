using BarebonesMessageBroker;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace UserModule.Tests;

public class UserWebApplicationFactory : WebApplicationFactory<Program>
{
    private static bool _databaseInitialized;
    private static readonly object _lock = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {
            var sp = services.BuildServiceProvider();
            services.AddSingleton<BarebonesMessageBroker.IBus>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                return new TestBus(scopeFactory);
            });

            
            using (var scope = sp.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                EnsureDatabaseCreated(dbContext);
            }

        });

    }
    private static void EnsureDatabaseCreated(UserDbContext context)
    {
        if (_databaseInitialized)
            return;

        lock (_lock)
        {
            context.Database.Migrate();
            _databaseInitialized = true;
        }
    }



}

