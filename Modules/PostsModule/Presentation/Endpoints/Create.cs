using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application;
using PostsModule.Application.Create;
using PostsModule.Domain.Auth;

namespace PostsModule.Presentation.Endpoints;
internal class Create
{
    internal static async Task<IResult> Process([FromServices] IRequestClient<CreatePostCommand> client, IAuthHelper auth, [FromBody] CreateBody body)
    {
        var command = new CreatePostCommand()
        {
            Title = body.Title,
            FactionName = body.FactionName,
            Description = body.Description,
            CreatorId = body.CreatorId,
            PrimaryColor = string.IsNullOrEmpty(body.PrimaryColor) ? string.Empty : body.PrimaryColor,
            SecondaryColor = string.IsNullOrEmpty(body.SecondaryColor) ? string.Empty : body.SecondaryColor,
        };

        try
        {
            var clientResponse = await client.GetResponse<CommandResult<string>>(command);

            if (clientResponse.Message.IsSuccess)
            {
                var token = auth.CreateToken(
                    DateTime.UtcNow.AddMinutes(5),
                    ClaimValueHolder.Create<string>("postId", clientResponse.Message.Result)
                    );
                return Results.Ok(new SuccessRespnse { PostId = clientResponse.Message.Result, Token = token });
            }

            return Results.StatusCode(clientResponse.Message.ResultStatus);
        }
        catch (Exception)
        {
            return Results.Problem();
        }
    }
}
public class SuccessRespnse
{
    public string PostId { get; set; }
    public string Token { get; set; }
}

