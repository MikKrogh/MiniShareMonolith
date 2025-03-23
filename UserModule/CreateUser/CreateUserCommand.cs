namespace UserModule.CreateUser;

public sealed record CreateUserCommand
{
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Password { get; init; } = string.Empty;
}

public sealed class CreateUserCommandHandler
{

}

public sealed record CreateUserCommandResult
{

}
public static  class CreateUserEndPoint
{
    public static async Task Process()
    {
    }
}