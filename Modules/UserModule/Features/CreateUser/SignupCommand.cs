namespace UserModule.Features.CreateUser;

public sealed record SignupCommand
{
    public string UserId { get; set; }
    public string UserName { get; set; }
}
