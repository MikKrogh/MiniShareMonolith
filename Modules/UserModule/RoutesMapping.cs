using MassTransit;
using Microsoft.AspNetCore.Mvc;
using UserModule.Features.CreateUser;
using UserModule.Features.GetUser;

namespace UserModule;

public static class RoutesMapping
{
    public static void AddUserModuleEndpoints(this IEndpointRouteBuilder builder)
    {
        var api = builder.MapGroup("/User").WithTags("UserModule");



        api.MapPost(string.Empty, async ([FromServices] IRequestClient<SignupCommand> client, [FromBody] SignupCommand body) =>
        {
            if (!Guid.TryParse(body.UserId, out _))
            {
                return Results.BadRequest();
            }
            var response = await client.GetResponse<SignupCommandResult>(body);

            if (response.Message.WasSucces)
                return Results.Ok();
            return Results.StatusCode(response.Message.StatusCode);
        })
        .WithSummary("Create a user")
        .Produces(200)
        .Produces(400)
        .Produces(500);

        api.MapGet("{id}", async ([FromServices] IRequestClient<GetUserCommand> client, string id) =>
        {
            var command = new GetUserCommand(id);
            var result = await client.GetResponse<GetUserCommandResult>(command);
            if (result?.Message?.User is null)
                return Results.NotFound();
            return Results.Ok(result.Message.User);
        })
        .WithSummary("Returns a user")
        .Produces<User>(200)
        .Produces(500);
    }
}
