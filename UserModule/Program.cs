
using UserModule;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapEndpoints();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();

public partial class Program { }