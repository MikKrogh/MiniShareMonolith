using UserModule.CreateUser;

namespace UserModule;

public static class EndpointsExtensions
{
    public static void MapEndpoints(this IEndpointRouteBuilder builder)
    {

        var api = builder.MapGroup("/api/User");

        api.MapPost(string.Empty, CreateUserEndPoint.Process);
    }
}
