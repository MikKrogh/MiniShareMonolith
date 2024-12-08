using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application.Get;

namespace PostsModule.Presentation.Endpoints;

internal class Get
{
	internal static async Task<IResult> Configure([FromServices] IRequestClient<GetPostCommand> client, [FromRoute]string postId) 
	{
		try
		{
			var command = new GetPostCommand();
			var clientResponse = await client.GetResponse<GetPostResult>(command);

			var dto = new PostDto
			{
				Id = clientResponse.Message.Id,
				Title = clientResponse.Message.Title,
				Description = clientResponse.Message.Description,
				CreatorId = clientResponse.Message.CreatorId,
				CreatorName = clientResponse.Message.CreatorName,
				PrimaryColor = clientResponse.Message.PrimaryColour,
				SecondaryColor = clientResponse.Message.SecondaryColour,
				CreationDate = clientResponse.Message.CreationDate
			};
			return Results.Ok(clientResponse);
		}
		catch (Exception)
		{
			return Results.Problem();
		}

	}
}
