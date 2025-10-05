using BarebonesMessageBroker;
using EngagementModule;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddEngagementModuleServices(builder.Configuration);
builder.Services.AddSingleton<BarebonesMessageBroker.IBus>(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    return new BareBonesBus(scopeFactory);
});


var app = builder.Build();
app.EngagementModuleEndpointSetup();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();

public partial class Program;