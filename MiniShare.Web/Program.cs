using PostsModule;
using PostsModule.Presentation;
using UserModule;
using EngagementModule;
using BarebonesMessageBroker;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddPostModuleServices(builder.Configuration);
builder.Services.AddEngagementModuleServices(builder.Configuration);
builder.Services.AddUserModuleServices(builder.Configuration);
builder.Services.AddSingleton<BarebonesMessageBroker.IBus>(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    return new BareBonesBus(scopeFactory);
});
builder.Logging.SetMinimumLevel(LogLevel.Error);
builder.Services.AddLogging();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseMetricServer();
app.UseHttpMetrics();

app.UseSwagger();
app.UseSwaggerUI();

app.AddPostModuleEndpoints();
app.AddUserModuleEndpoints();
app.EngagementModuleEndpointSetup();

app.Run();
