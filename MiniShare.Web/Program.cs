using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using MassTransit;
using OpenTelemetry.Metrics;
using PostsModule;
using PostsModule.Presentation;
using UserModule;

var builder = WebApplication.CreateBuilder(args);


if (builder.Environment.IsProduction()) 
{
    builder.PostModuleAppConfiguration();
    builder.UserModuleAppConfiguration();
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(new Uri(builder.Configuration["AppConfigEndpoint"]), new DefaultAzureCredential())
        .Select("monolith*").TrimKeyPrefix("monolith:");
    });
}

builder.Services.AddPostModuleServices(builder.Configuration);
builder.Services.AddUserModuleServices(builder.Configuration);
builder.Services.AddLogging();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(PostsModule.ServiceExtensions).Assembly);
    x.AddConsumers(typeof(UserModule.ServiceExtensions).Assembly);
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenTelemetry()
    .UseAzureMonitor(options =>
    {
        options.ConnectionString = builder.Configuration["ApplicationInsightsConnectionString"];
    })
    .WithMetrics(options =>
    {
        options.AddAspNetCoreInstrumentation();
    }
);       
    

var app = builder.Build();

var loger = app.Services.GetRequiredService<ILogger<Program>>();
loger.LogCritical("Log critical...");
loger.LogError("Log error...");
loger.LogWarning("Log warning...");
loger.LogInformation("Log information...");
loger.LogDebug("Log debug...");
loger.LogTrace("Log trace...");


app.UseSwagger();
app.UseSwaggerUI();
app.AddPostModuleEndpoints();
app.AddUserModuleEndpoints();
app.Run();

public partial class Program { }