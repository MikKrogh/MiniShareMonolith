using PostsModule.Application;
using PostsModule.Presentation.Endpoints;

namespace PostsModule.Presentation;

public static class EndpointsExtensions
{
    public static void AddPostModuleEndpoints(this IEndpointRouteBuilder routeBuilder)
    {

        var api = routeBuilder.MapGroup("/Posts").WithTags("PostModule");
        
        api.MapGet("/{postId}", GetPost.Process)
            .WithSummary("Returns details of a post")
            .Produces<PostDto>(200)
            .Produces(404)
            .Produces(500);

        api.MapGet("", GetPosts.Process)
            .WithSummary("Returns pagination result of posts, ")
            .WithDescription("Follows Odata structure for search, filtering and ordering")
            .Produces<List<PostDto>>(200)
            .Produces(500);

        api.MapPost(string.Empty, CreatePost.Process)
            .WithSummary("Create a post")
            .Produces(200)
            .Produces(400)
            .Produces(500);

        api.MapDelete("{postId}", DeletePost.Process)
            .WithSummary("Delete a post and images related to post")
            .Produces(200)
            .Produces(500);
    }
}