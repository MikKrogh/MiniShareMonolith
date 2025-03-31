using PostsModule;
using PostsModule.Presentation;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AppConfiguration();

builder.Services.AddPostModuleServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.AddPostModuleEndpoints();
app.Run();


public partial class Program { }