
namespace UserModule.Features.GetUser;

internal class GetUserCommandHandler
{
    private readonly IUserRepository repository;

    public GetUserCommandHandler(IUserRepository repository)
    {
        this.repository = repository;
    }

    public async Task<GetUserCommandResult> Consume(GetUserCommand context)
    {
        var user = await repository.GetUser(context.UserId);
        if (user is null)
            return new GetUserCommandResult() { User = null };
        return new GetUserCommandResult() { User = user };
    }
}
