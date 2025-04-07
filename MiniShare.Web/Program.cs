using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PostsModule;
using PostsModule.Presentation;
using System.Security.Cryptography;
using System.Text;
using UserModule;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.PostModuleAppConfiguration();
builder.Configuration.UserModuleAppConfiguration();
await builder.Services.AddPostModuleServices(builder.Configuration);
builder.Services.AddUserModuleServices(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(PostsModule.ServiceExtensions).Assembly);
    x.AddConsumers(typeof(UserModule.ServiceExtensions).Assembly);
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        // Configure token validation parameters
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidIssuer = "https://securetoken.google.com/userservice-37984",
//            ValidateAudience = true,
//            ValidAudience = "userservice-37984",
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            // You can add the RSA keys manually. Here’s an example for one key:
//            IssuerSigningKey = new RsaSecurityKey(new RSAParameters
//            {
//                Modulus = Base64UrlEncoder.DecodeBytes("pfNhqJgt_MJ94sQgdO1RFYgUuxRSz6DLXiFkjLl7vNPtueNFURD051m9OTwfonCzpuMPzqn_4KLsb4CPhC4ZAq__wBy3wy7GapQffHZkFhnHuAM1EAaCXDdgUg5soc_jBbFhl2woNw37ls7ZAgcUadrgCECMJJbhfWz1cEJKnO0tKNGg4hpGgExE_08vikhDkHlpbiCOUyjFWLi0PYi4mZRrahsLZ1VqcnmCJ4LO6l9AQWZtmeZM6tihou8x_33tIQPwy_WhRP0YoT8EHMXu-w2yEYnpFRIfqkIJNolaHVbbujk64zFd2jBgKucKFyB68dXldq17_ITHpSjsDCZWjw"),
//                Exponent = Base64UrlEncoder.DecodeBytes("AQAB")
//            })
//            // To add multiple keys, you can use IssuerSigningKeys collection.
//        };
//    });

//builder.Services.AddAuthorization();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
//app.UseAuthentication();
//app.UseAuthorization();

app.AddPostModuleEndpoints();
app.AddUserModuleEndpoints();



app.Run();


public partial class Program { }