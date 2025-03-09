using PostsModule;
using PostsModule.Presentation;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AppConfiguration();

builder.Services.AddPostsServiceExtensions(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.AddPostsEndpoints();



app.Run();
