namespace UserModule.Features.CreateUser;

public sealed record SignupCommand
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}
