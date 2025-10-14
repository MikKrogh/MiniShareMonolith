namespace UserModule.Features.GetUsers;

internal class GetUsersCommandHandler
{
    private readonly IUserRepository repo;

    public GetUsersCommandHandler(IUserRepository repo)
    {
        this.repo = repo;
    }
    public async Task<GetUsersCommandResult> Consume(GetUsersCommand command)
    {
        var users = await repo.GetUsers(command.ids);
        var result = new GetUsersCommandResult()
        {
            Users = users.ToList()
        };
        
        return result;
    }
}

internal record GetUsersCommand(List<string> ids);
internal record GetUsersCommandResult
{
    public List<User> Users { get; init; } = new List<User>();
}