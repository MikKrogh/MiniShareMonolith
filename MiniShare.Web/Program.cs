using PostsModule;
using PostsModule.Presentation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddPostsServiceExtensions(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsProduction())
{

}

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.AddPostsEndpoints();



app.Run();
