using System.Net;

namespace PostsModule.Application;

public sealed class CommandResult<T> : AbstractCommandResult where T : class
{
    public T? Result { get; init; } = null;

    public static CommandResult<T> FailedToValidate() => new CommandResult<T>
    {
        IsSuccess = false,
        Result = default,
        ResultStatus = (int)HttpStatusCode.BadRequest
    };
    public static CommandResult<T> InternalError() => new CommandResult<T>
    {
        IsSuccess = false,
        Result = default,
        ResultStatus = (int)HttpStatusCode.InternalServerError
    };
    public static CommandResult<T> Success(T result)
    {
        if (result is null) throw new Exception("cant have a successfull create with a null value result");

        return new CommandResult<T>
        {
            Result = result,
            IsSuccess = true,
            ResultStatus = (int)HttpStatusCode.OK
        };
    }
}
public sealed class CommandResult : AbstractCommandResult
{
    public static CommandResult FailedToValidate() => new CommandResult
    {
        IsSuccess = false,
        ResultStatus = (int)HttpStatusCode.BadRequest
    };
    public static CommandResult Success() => new CommandResult
    {
        IsSuccess = true,
        ResultStatus = (int)HttpStatusCode.OK
    };
    public static CommandResult InternalError() => new CommandResult
    {
        IsSuccess = false,
        ResultStatus = (int)HttpStatusCode.InternalServerError
    };

}

public abstract class AbstractCommandResult
{
    public bool IsSuccess { get; init; } = false;
    public int ResultStatus { get; init; } = -1;
}