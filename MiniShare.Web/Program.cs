using BarebonesMessageBroker;
using EngagementModule;
using OpenTelemetry.Metrics;
using PostsModule;
using PostsModule.Presentation;
using Prometheus;
using UserModule;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddPostModuleServices(builder.Configuration);
builder.Services.AddEngagementModuleServices(builder.Configuration);
builder.Services.AddUserModuleServices(builder.Configuration);

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation();
    });

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


app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";      
    });
});

app.UseMetricServer();
app.UseSwagger();
app.UseSwaggerUI();

app.AddPostModuleEndpoints();
app.AddUserModuleEndpoints();
app.EngagementModuleEndpointSetup();

app.Run();
