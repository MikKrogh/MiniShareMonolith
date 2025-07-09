using MassTransit;

namespace UserModule.Features.CreateUser;

public sealed class SignupCommandHandler : IConsumer<SignupCommand>
{
    private readonly IUserRepository repository;
    private readonly IBus sender;
    private readonly BarebonesMessageBroker.IBus tmpBus;

    public SignupCommandHandler(IUserRepository repository, IBus sender, BarebonesMessageBroker.IBus tmpBus)
    {
        this.repository = repository;
        this.sender = sender;
        this.tmpBus = tmpBus;
    }
    public async Task Consume(ConsumeContext<SignupCommand> context)
    {

        if (!IsValidCommand(context.Message))
        {
            await context.RespondAsync(SignupCommandResult.BadRequest());
            return;
        }
        try
        {
            var user = new User
            {
                UserName = context.Message.DisplayName,
                Id = context.Message.UserId,
                CreationDate = DateTime.UtcNow
            };
            await repository.CreateUser(user);
            //await SendUserCreatedEvent(user);
            await tmpBus.Publish(new EventMessages.UserCreatedEvent()
            {
                UserId = user.Id,
                UserName = user.UserName
            }, "UserModule.UserCreated");
            await context.RespondAsync(SignupCommandResult.Success());
        }
        catch (Exception e)
        {
            await context.RespondAsync(SignupCommandResult.InternalError());
        }
    }

    private async Task SendUserCreatedEvent(User user)
    {
        await sender.Publish(new EventMessages.UserCreatedEvent()
        {
            UserId = user.Id,
            UserName = user.UserName
        });
    }

    private bool IsValidCommand(SignupCommand command)
    {
        return !string.IsNullOrEmpty(command.UserId) && !string.IsNullOrEmpty(command.DisplayName);
    }
}
