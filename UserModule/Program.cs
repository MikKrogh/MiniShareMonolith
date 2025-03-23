
using UserModule;
using UserSignupModule;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddUserSignupServiceExtensions(builder.Configuration);


var app = builder.Build();
app.MapEndpoints();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.Run();

public partial class Program { }