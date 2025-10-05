using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using PostsModule;
using PostsModule.Presentation;
using PostsModule.Presentation.Endpoints;
using UserModule;
using UserModule.OpenTelemetry;
using EngagementModule;
using BarebonesMessageBroker;

var builder = WebApplication.CreateBuilder(args);


if (builder.Environment.IsProduction()) 
{
    builder.PostModuleAppConfiguration();
    builder.UserModuleAppConfiguration();
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(new Uri(builder.Configuration["AppConfigEndpoint"]), new DefaultAzureCredential())
        .Select("monolith*").TrimKeyPrefix("monolith:")
        .Select("EngagementModule").TrimKeyPrefix("EngagementModule:")
        .Select("PostModuleModule").TrimKeyPrefix("EngagementModule:");
    });
}

builder.Services.AddPostModuleServices(builder.Configuration);
builder.Services.AddEngagementModuleServices(builder.Configuration);
builder.Services.AddUserModuleServices(builder.Configuration);
builder.Services.AddSingleton<BarebonesMessageBroker.IBus>(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    return new BareBonesBus(scopeFactory);
});
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Services.AddLogging();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
if (builder.Environment.IsProduction())
{
    builder.Services.AddOpenTelemetry()
    .UseAzureMonitor(options =>
    {
        if (string.IsNullOrEmpty(builder.Configuration["ApplicationInsightsConnectionString"]))
            throw new Exception("no connection string for applicationInsigts");

        options.ConnectionString = builder.Configuration["ApplicationInsightsConnectionString"];

    })
    .WithMetrics(options =>
    {
        options.AddMeter(UserCreatedMeter.MeterName);
        options.AddMeter(PostCreatedMeter.MeterName);
        options.AddAspNetCoreInstrumentation();
    }
);
    builder.Logging.AddOpenTelemetry(x => x.AddOtlpExporter());
}

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.AddPostModuleEndpoints();
app.AddUserModuleEndpoints();
app.EngagementModuleEndpointSetup();
app.Run();

public partial class Program { }