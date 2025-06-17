using EngagementModule;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddEngagementModuleServices(builder.Configuration);


var app = builder.Build();

var rootApi = app.MapGroup("/engagement/{postid}").WithTags("engagementModule");
var likesApi = rootApi.MapGroup("/likes").WithTags("likes");

likesApi.MapPost(string.Empty, async (string postid, [FromQuery]string userId, [FromServices] IPostLikeService service) =>
{
	try
	{
        await service.Like(postid, userId);
        return Results.Ok();
    }
    catch (Exception e)
	{
        if (e.InnerException is PostgresException pgex && pgex.SqlState == Constants.PostgresDublicateErrorCode)         
            return Results.Ok();        
        else return Results.Problem();
	}

});
likesApi.MapDelete(string.Empty, async (string postid, [FromQuery]string userId, IPostLikeService service) =>
{
    await service.Unlike(postid, userId);
    return Results.Ok(new { UserId = userId, PostId = postid });
});
likesApi.MapGet("/count", async (string postid, IPostLikeService service) =>
{    
    var count = await service.GetLikesCount(postid);
    return Results.Ok(new {Count = count });
});


app.UseSwagger();
app.UseSwaggerUI();
app.Run();

public partial class Program;