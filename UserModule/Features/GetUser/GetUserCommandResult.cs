namespace UserModule.Features.GetUser;

internal record GetUserCommandResult
{
    public User? User { get; set; }
}

