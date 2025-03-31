using UserModule;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddUserModuleServices(builder.Configuration);


var app = builder.Build();
app.AddUserModuleEndpoints();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.Run();

public partial class Program { }