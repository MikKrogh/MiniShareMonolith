using PostsModule;
using PostsModule.Presentation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddPostsServiceExtensions();

var app = builder.Build();

app.AddPostsEndpoints();
app.Run();


public partial class Program{}