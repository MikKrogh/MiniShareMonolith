using PostsModule;
using PostsModule.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPostModuleServices(builder.Configuration);
builder.Services.Configure<HostOptions>(options =>
{
    options.ShutdownTimeout = TimeSpan.FromSeconds(15);
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.AddPostModuleEndpoints();
app.Run();


public partial class Program { }