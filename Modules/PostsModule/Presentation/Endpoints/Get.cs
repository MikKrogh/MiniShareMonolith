using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application.Get;

namespace PostsModule.Presentation.Endpoints;

internal class Get
{
	internal static async Task<IResult> Process([FromServices] IRequestClient<GetPostCommand> client, [FromRoute]string postId) 
	{
		try
		{
			var command = new GetPostCommand(postId);
			var clientResponse = await client.GetResponse<GetPostResult>(command);

			var dto = new PostDto
			{
				Id = clientResponse.Message.Id,
				Title = clientResponse.Message.Title,
				Description = clientResponse.Message.Description,
				CreatorId = clientResponse.Message.CreatorId,
				CreatorName = clientResponse.Message.CreatorName,
				Images = clientResponse.Message.Images,
				PrimaryColor = clientResponse.Message.PrimaryColor,
				SecondaryColor = clientResponse.Message.SecondaryColor,
			};
			return Results.Ok(dto);
		}
		catch (Exception)
		{
			return Results.Problem();
		}

	}
}
