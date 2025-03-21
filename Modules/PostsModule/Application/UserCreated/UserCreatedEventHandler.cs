using MassTransit;
using PostsModule.Domain;

namespace PostsModule.Application.UserEvents;

public sealed class UserCreatedEventHandler : IConsumer<UserCreatedEvent>
{
    private readonly IUserRepository _userRepository;
    public UserCreatedEventHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        try
        {
            var user = User.Create(context.Message.UserId);
            user.SetName(context.Message.UserName);
            await _userRepository.Create(user);
        }
        catch (Exception e)
        {

            throw;
        }
    }
}

