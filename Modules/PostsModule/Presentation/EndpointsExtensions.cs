using PostsModule.Application;
using PostsModule.Presentation.Endpoints;

namespace PostsModule.Presentation;

public static class EndpointsExtensions
{
    public static void AddPostModuleEndpoints(this IEndpointRouteBuilder routeBuilder)
    {

        var api = routeBuilder.MapGroup("/Posts");

        api.MapGet("/{postId}", GetPost.Process).WithDescription("This endpoint returns a jsonbody for a post")
            .WithSummary("Gets a single post")
            .WithTags("Read")
            .Produces<PostDto>(200)
            .Produces(500);

        api.MapGet("", GetPosts.Process)
            .WithSummary("returns collection of posts")
            .WithSummary("Follows Odata structure for search, filtering and ordering")
            .WithTags("Read")
            .Produces<List<PostDto>>()
            .Produces(500);

        api.MapGet("{postId}/Image/{ImageId}", GetImage.Process)
            .Produces(200)
            .Produces(404)
            .Produces(500)
            .WithTags("Read")
            .WithSummary("Returns an image from blob storage");

        api.MapPost(string.Empty, CreatePost.Process)
            .WithDescription("This endpoint expects a jsonbody with a post and images")
            .WithSummary("create a post, containing content")
            .WithTags("Write")
            .Produces(200)
            .Produces(400)
            .Produces(500);

        api.MapPut("{postId}/Image", AddImage.Process)
            .Produces(200)
            .WithTags("Write")
            .WithSummary("takes an image to blob storage, and updates the postEntity to know about the image")
            .Produces(500)
            .DisableAntiforgery();
    }
}

