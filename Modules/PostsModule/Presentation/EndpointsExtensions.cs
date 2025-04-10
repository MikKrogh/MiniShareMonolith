using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application;
using PostsModule.Application.Create;
using PostsModule.Domain.Auth;
using PostsModule.Presentation.Endpoints;

namespace PostsModule.Presentation;

public static class EndpointsExtensions
{
    public static void AddPostModuleEndpoints(this IEndpointRouteBuilder routeBuilder)
    {

        var api = routeBuilder.MapGroup("/Posts").WithTags("PostModule");

        api.MapGet("Test", Tes.Process);

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

        api.MapGet("{postId}/Image/{ImageId}", GetImage.Process)
            .Produces(200)
            .Produces(404)
            .Produces(500)
            .WithSummary("Returns an image file");

        api.MapPost(string.Empty, CreatePost.Process)
            .WithSummary("Create a post")
            .Produces(200)
            .Produces(400)
            .Produces(500);

        api.MapPut("{postId}/Image", AddImage.Process)
            .Produces(200)
            .WithSummary("Appends image file to a post")
            .Produces(500)
            .DisableAntiforgery();
    }
}


public static class Tes
{
    public static async  Task<IResult> Process([FromServices] IRequestClient<CreatePostCommand> client, IAuthHelper auth, [FromBody] CreateBody body)
    {
        return Results.Ok();
    }
}