using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PostsModule.Presentation.Endpoints;

namespace PostsModule.Presentation;

public static class EndpointsExtensions
{
	public static void AddPostsEndpoints(this IEndpointRouteBuilder routeBuilder)
	{
		var api = routeBuilder.MapGroup("/Posts");

		//api.MapPost(string.Empty, async (HttpRequest request) => 
		api.MapPost(string.Empty,  (HttpRequest request) => 
        {
            var form = await request.ReadFormAsync(); // Read only the form fields, not the streams

            UploadRequest? title = System.Text.Json.JsonSerializer.Deserialize< UploadRequest>(form["entity"]);
            var fileCollection = form.Files;
            Console.WriteLine();

        }).DisableAntiforgery();


        //api.MapPost(string.Empty, Create.Process)
        //	.WithDescription("This endpoint expects a jsonbody with a post and images")
        //	.WithSummary("create a post with content")
        //	.WithTags("Command")
        //	.Produces(200)
        //	.Produces(400)
        //	.Produces(500)
        //	.DisableAntiforgery(); // should not be disabled?

        api.MapGet("/{postId}", Get.Process).WithDescription("This endpoint returns a jsonbody for a post")
			.WithSummary("Gets a single post")
			.WithTags("Query")
			.Produces<PostDto>(200)
			.Produces(500);
	}

}

public class UploadRequest
{
    public string Title { get; set; }

}