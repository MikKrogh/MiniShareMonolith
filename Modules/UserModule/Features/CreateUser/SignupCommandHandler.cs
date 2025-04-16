using MassTransit;

namespace UserModule.Features.CreateUser;

public sealed class SignupCommandHandler : IConsumer<SignupCommand>
{
    private readonly IUserRepository repository;
    private readonly IBus sender;

    public SignupCommandHandler(IUserRepository repository, IBus sender)
    {
        this.repository = repository;
        this.sender = sender;
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
                UserName = context.Message.UserName,
                Id = context.Message.UserId,
                CreationDate = DateTime.UtcNow
            };
            await repository.CreateUser(user);
            await SendUserCreatedEvent(user);
            await context.RespondAsync(SignupCommandResult.Success());
        }
        catch (Exception)
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
        return !string.IsNullOrEmpty(command.UserId) && !string.IsNullOrEmpty(command.UserName);
    }
}
