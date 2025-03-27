using MassTransit;
namespace UserModule.Features.GetUser;

internal class GetUserCommandHandler : IConsumer<GetUserCommand>
{
    private readonly IUserRepository repository;

    public GetUserCommandHandler(IUserRepository repository)
    {
        this.repository = repository;
    }

    public async Task Consume(ConsumeContext<GetUserCommand> context)
    {
        var user = await repository.GetUser(context.Message.UserId);

        if (user is null)
            await context.RespondAsync(new GetUserCommandResult() { User = null});
         await context.RespondAsync(new GetUserCommandResult() { User = user});
    }
}
