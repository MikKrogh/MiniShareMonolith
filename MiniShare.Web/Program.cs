using PostsModule;
using PostsModule.Presentation;
using UserModule;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AppConfiguration();

builder.Services.AddPostModuleServices(builder.Configuration);
builder.Services.AddUserModuleServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.AddPostModuleEndpoints();
app.AddUserModuleEndpoints();



app.Run();


public partial class Program { }