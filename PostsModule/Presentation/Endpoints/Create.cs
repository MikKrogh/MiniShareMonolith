using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application.Create;

namespace PostsModule.Presentation.Endpoints;
internal class Create
{
	internal static async Task<IResult> Configure([FromServices] IRequestClient<CreatePostCommand> client, [FromBody] CreateBody body)
	{
		try
		{
			var command = new CreatePostCommand()
			{
				Title = body.Title,
				Description = body.Description,
				CreatorId = body.CreatorId,
				PrimaryColor = body.PrimaryColour,
				SecondaryColor = body.SecondaryColour,
			};
			var clientResponse = await client.GetResponse<CreatePostResult>(command);

			return Results.Ok(clientResponse.Message);
		}
		catch (Exception)
		{
			return Results.Problem();
		}
	}
}
