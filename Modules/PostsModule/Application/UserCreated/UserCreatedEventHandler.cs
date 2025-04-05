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
            Guid id;
            var couldParse = Guid.TryParse(context.Message.UserId, out id);
            if (!couldParse) throw new Exception("could not handle usercreatedevent becouse id is not guid");



            var user = User.Create(id);
            user.SetName(context.Message.UserName);
            await _userRepository.Create(user);
        }
        catch (Exception e)
        {

            throw;
        }
    }
}

