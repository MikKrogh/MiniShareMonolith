using BarebonesMessageBroker;

using UserModule;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddUserModuleServices(builder.Configuration);

builder.Services.AddSingleton<BarebonesMessageBroker.IBus>(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    return new BareBonesBus(scopeFactory);
});

var app = builder.Build();
app.AddUserModuleEndpoints();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.Run();

public partial class Program { }