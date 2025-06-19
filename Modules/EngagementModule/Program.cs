using EngagementModule;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddEngagementModuleServices(builder.Configuration);

var app = builder.Build();
app.EngagementModuleEndpointSetup();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();

public partial class Program;