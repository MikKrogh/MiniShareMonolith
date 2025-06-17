using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using MassTransit;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using PostsModule;
using PostsModule.Presentation;
using PostsModule.Presentation.Endpoints;
using UserModule;
using UserModule.OpenTelemetry;
using EngagementModule;

var builder = WebApplication.CreateBuilder(args);


if (builder.Environment.IsProduction()) 
{
    builder.PostModuleAppConfiguration();
    builder.UserModuleAppConfiguration();
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(new Uri(builder.Configuration["AppConfigEndpoint"]), new DefaultAzureCredential())
        .Select("monolith*").TrimKeyPrefix("monolith:")
        .Select("EngagementModule");
    });
}

builder.Services.AddPostModuleServices(builder.Configuration);
builder.Services.AddEngagementModuleServices(builder.Configuration);
builder.Services.AddUserModuleServices(builder.Configuration);
builder.Services.AddLogging();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(PostsModule.ServiceExtensions).Assembly);
    x.AddConsumers(typeof(UserModule.ServiceExtensions).Assembly);
    x.AddConfigureEndpointsCallback((context, name, cfg) =>
    {
        cfg.UseMessageRetry(r => r.Immediate(5));

    });
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

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
app.Run();

public partial class Program { }