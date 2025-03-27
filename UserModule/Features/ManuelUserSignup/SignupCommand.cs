namespace UserModule.Features.ManuelUserSignup;

public sealed record SignupCommand
{
    public string UserId { get; set; }
    public string UserName { get; set; }
}
