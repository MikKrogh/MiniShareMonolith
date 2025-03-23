namespace UserModule.Features.ManuelUserSignup;

internal sealed record SignupCommandResult
{
    public int StatusCode { get; init; } = -1;
    public bool WasSucces { get; init; } = false;

    public static SignupCommandResult Success() =>  new SignupCommandResult() { StatusCode = 200, WasSucces = true };
    public static SignupCommandResult BadRequest() =>  new SignupCommandResult() { StatusCode = 400, WasSucces = false };
    public static SignupCommandResult InternalError() =>  new SignupCommandResult() { StatusCode = 500, WasSucces = false };
}
