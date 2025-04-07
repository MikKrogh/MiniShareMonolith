using MassTransit;
using UserModule;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddUserModuleServices(builder.Configuration);
builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(ServiceExtensions).Assembly);
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();
app.AddUserModuleEndpoints();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.Run();

public partial class Program { }