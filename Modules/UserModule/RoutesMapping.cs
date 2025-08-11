
using Microsoft.AspNetCore.Mvc;
using UserModule.Features.CreateUser;
using UserModule.Features.GetUser;
using UserModule.Features.GetUsers;
using UserModule.OpenTelemetry;

namespace UserModule;

public static class RoutesMapping
{
    public static void AddUserModuleEndpoints(this IEndpointRouteBuilder builder)
    {
        var api = builder.MapGroup("/User").WithTags("UserModule");

        api.MapPost(string.Empty, async ([FromServices]SignupCommandHandler handler,[FromBody] SignupCommand body, [FromQuery]string UserId) =>
        {
            var command = new SignupCommand
            {
                UserId = UserId,
                DisplayName = body.DisplayName
            };
            var response = await handler.Handle(command);

            if (response.WasSucces)
            {
                UserCreatedMeter.UserCreatedCounter.Add(1, new KeyValuePair<string, object>("user_role", "admin"));
                return Results.Ok();
            }
            return Results.StatusCode(response.StatusCode);
        })
        .WithSummary("Create a user")
        .Produces(200)
        .Produces(400)
        .Produces(500);

        api.MapGet("{id}", async ([FromServices] GetUserCommandHandler handler, string id) =>
        {
            var command = new GetUserCommand(id);
            var result = await handler.Consume(command);
            if (result?.User is null)
                return Results.NotFound();
            return Results.Ok(result.User);
        })
        .WithSummary("Returns a user")
        .Produces<User>(200)
        .Produces(500);

        var usersApi = builder.MapGroup("/Users").WithTags("UserModule");

        usersApi.MapGet("{userIds}", async ([FromRoute] string userIds, [FromServices] GetUsersCommandHandler handler) =>
        {
            List<string> ids = userIds.Split(',').ToList();
            var command = new GetUsersCommand(ids);
            var result = await handler.Consume(command);
            return Results.Ok(result.Users);
        });
    }
}
