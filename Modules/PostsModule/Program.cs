using PostsModule;
using PostsModule.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AppConfiguration();
builder.Services.AddPostsServiceExtensions(builder.Configuration);

var app = builder.Build();

app.AddPostsEndpoints();
app.Run();


public partial class Program { }