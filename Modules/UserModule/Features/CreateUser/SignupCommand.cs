namespace UserModule.Features.CreateUser;

public sealed record SignupCommand
{
    public string UserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
