
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using UserModule.Features.ManuelUserSignup;

namespace UserModule;

public static class RoutesMapping
{
    public static void MapEndpoints(this IEndpointRouteBuilder builder)
    {

        var api = builder.MapGroup("/User");

        api.MapPost(string.Empty, async ([FromServices] IRequestClient<SignupCommand> client, [FromBody] SignupCommand body) => 
        {
            var response = await client.GetResponse<SignupCommandResult>(body);

            if (response.Message.WasSucces)            
                return Results.Ok();            
            return Results.StatusCode(response.Message.StatusCode);
        });
    }
}
