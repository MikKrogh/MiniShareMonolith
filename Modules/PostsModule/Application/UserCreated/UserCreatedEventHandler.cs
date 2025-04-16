using EventMessages;
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
            
            if(string.IsNullOrEmpty(context.Message.UserId) || string.IsNullOrEmpty(context.Message.UserName))
                throw new Exception("could not handle usercreatedevent becouse id or name is null or empty");


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

