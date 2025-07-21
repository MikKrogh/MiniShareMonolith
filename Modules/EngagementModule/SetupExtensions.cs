using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace EngagementModule;

public static class SetupExtensions
{
    public static void AddEngagementModuleServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<EngagementDbContext>(options => options.EnableSensitiveDataLogging(false));
        serviceCollection.AddTransient<IPostLikeService, EngagementDbContext>();
        serviceCollection.AddScoped<ICommentService, EngagementDbContext>();
        serviceCollection.AddScoped<ChainActivityService, EngagementDbContext>();
    }
    public static void EngagementModuleEndpointSetup(this IEndpointRouteBuilder builder) 
    {
        var rootApi = builder.MapGroup("/engagement").WithTags("engagementModule");

        var likesRoutes = rootApi.MapGroup("{postid}/likes").WithTags("likes");
        likesRoutes.MapPost(string.Empty, async (string postid, [FromQuery] string userId, [FromServices] IPostLikeService service) =>
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
        likesRoutes.MapDelete(string.Empty, async (string postid, [FromQuery] string userId, IPostLikeService service) =>
        {
            await service.Unlike(postid, userId);
            return Results.Ok(new { UserId = userId, PostId = postid });
        });
        likesRoutes.MapGet("/count", async (string postid, IPostLikeService service) =>
        {
            var count = await service.GetLikesCount(postid);
            return Results.Ok(new { Count = count });
        });
        likesRoutes.MapGet(string.Empty, async (string postid, [FromQuery] string userId, IPostLikeService service) =>
        {
            var hasLiked = await service.HasLiked(postid, userId);
            return Results.Ok(hasLiked);
        });

        var commentsRoutes = rootApi.MapGroup("{postid}/comments").WithTags("engagementModule");
        commentsRoutes.MapPost(string.Empty, Comments.AddComment.Process);
        commentsRoutes.MapGet(string.Empty, Comments.GetComments.Process);
        

        var notificationRoutes = rootApi.MapGroup("/notifications").WithTags("engagementModule");
        notificationRoutes.MapGet(string.Empty, Notification.GetNotifications.Process);
        notificationRoutes.MapPost(string.Empty, Notification.UpdateSyncronizationTime.Process);
        

        
    }
}
