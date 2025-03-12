
namespace PostsModule.Application;

public sealed class CommandResult<T> where T: class, new()
{
    public bool IsSuccess { get; init; } = false;
    public int ResultStatus { get; init; } = -1;
    public T? ResultValue { get; init; } = default;

    public static CommandResult<T> FailedToValidate() 
    {
        return new CommandResult<T>()
        {
            IsSuccess = false,
            ResultStatus = 400,
            ResultValue = null
        };
    }

    public static CommandResult<T> InternalError()
    {
        return new CommandResult<T>()
        {
            IsSuccess = false,
            ResultStatus = 500,
            ResultValue = null
        };
    }

    public static CommandResult<T> Success(T result)
    {
        return new CommandResult<T>()
        {
            IsSuccess = true,
            ResultStatus = 200,
            ResultValue = result
        };
    }
}
