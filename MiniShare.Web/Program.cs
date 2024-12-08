using PostsModule;
using PostsModule.Presentation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddPostsServiceExtensions();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.AddPostsEndpoints();



app.Run();
