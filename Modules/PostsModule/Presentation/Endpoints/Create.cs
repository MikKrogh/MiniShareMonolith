using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application.Create;

namespace PostsModule.Presentation.Endpoints;
internal class Create
{
	internal static async Task<IResult> Process([FromServices] IRequestClient<CreatePostCommand> client, [FromBody] CreateBody body)
	{
		var command = new CreatePostCommand()
		{
			Title = body.Title,
			FactionName = body.FactionName,
			Description = body.Description,
			CreatorId = body.CreatorId,
			PrimaryColor = string.IsNullOrEmpty(body.PrimaryColor) ? string.Empty : body.PrimaryColor,
			SecondaryColor = string.IsNullOrEmpty(body.SecondaryColor) ? string.Empty : body.SecondaryColor,
			Images = body.Images ?? new FormFileCollection()
        };

		try
		{
            var clientResponse = await client.GetResponse<CreatePostResult>(command);

			if (clientResponse.Message.IsSuccess)
				return Results.Ok(new {PostId = clientResponse.Message.PostId });
            return Results.StatusCode(clientResponse.Message.ResultStatus);
        }
		catch (Exception)
		{
			return Results.Problem();
        }

	}
}
