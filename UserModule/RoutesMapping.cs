using MassTransit;
using MassTransit.Clients;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using UserModule.Features.GetUser;
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

        api.MapGet("{id}", async ([FromServices] IRequestClient<GetUserCommand> client, string id) => 
        {
            var command = new GetUserCommand(id);
            var result = await client.GetResponse<GetUserCommandResult>(command);
            if (result?.Message?.User is null)
                return Results.NotFound();
            return Results.Ok(result.Message.User);
            

            
        });

        
    }
}
