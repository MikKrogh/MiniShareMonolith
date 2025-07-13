using BarebonesMessageBroker;
using PostsModule.Domain;

namespace PostsModule.Application.UserEvents;

public sealed class UserCreatedEventHandler :  Listener<UserCreatedEvent>

{
    private readonly IUserRepository _userRepository;
    public UserCreatedEventHandler(IUserRepository userRepository)    
    {
        _userRepository = userRepository;
    }

    public async Task Handle(UserCreatedEvent t)
    {
        try
        {
            if (string.IsNullOrEmpty(t.UserId) || string.IsNullOrEmpty(t.UserName))
                throw new Exception("could not handle usercreatedevent becouse id or name is null or empty");

            var user = User.Create(t.UserId);
            user.SetName(t.UserName);
            await _userRepository.Create(user);
        }
        catch (Exception e)
        {

            throw;
        }        
    }



}

public sealed class UserCreatedEvent : BarebonesMessageBroker.Event
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public string EventName => "UserModule.UserCreated";

    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
}