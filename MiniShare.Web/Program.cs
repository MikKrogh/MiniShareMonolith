using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PostsModule;
using PostsModule.Presentation;
using System.Security.Cryptography;
using System.Text;
using UserModule;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.PostModuleAppConfiguration();
builder.Configuration.UserModuleAppConfiguration();
await builder.Services.AddPostModuleServices(builder.Configuration);
builder.Services.AddUserModuleServices(builder.Configuration);

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

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.AddPostModuleEndpoints();
app.AddUserModuleEndpoints();



app.Run();


public partial class Program { }