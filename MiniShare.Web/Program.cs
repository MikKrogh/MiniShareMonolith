using Azure.Monitor.OpenTelemetry.AspNetCore;
using MassTransit;
using OpenTelemetry.Metrics;
using PostsModule;
using PostsModule.Presentation;

using UserModule;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.PostModuleAppConfiguration();
builder.Configuration.UserModuleAppConfiguration();
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
        options.ConnectionString = "InstrumentationKey=6a0c38b7-2a1c-4945-b272-e4d491be7b41;IngestionEndpoint=https://northeurope-2.in.applicationinsights.azure.com/;LiveEndpoint=https://northeurope.livediagnostics.monitor.azure.com/;ApplicationId=a039a579-9d41-411e-8a7a-335a09747e7c";
    })
    .WithMetrics(options =>
    {
        options.AddAspNetCoreInstrumentation();
    }
);       
    

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.AddPostModuleEndpoints();
app.AddUserModuleEndpoints();
app.Run();

public partial class Program { }