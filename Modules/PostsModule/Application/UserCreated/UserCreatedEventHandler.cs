using BarebonesMessageBroker;
using PostsModule.Domain;

namespace PostsModule.Application.UserEvents;

public sealed class UserCreatedEventHandler :  Listener<TmpEventUSer>

{
    private readonly IUserRepository _userRepository;
    public UserCreatedEventHandler(IUserRepository userRepository)    
    {
        _userRepository = userRepository;
    }

    public async Task Handle(TmpEventUSer t)
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

public sealed class TmpEventUSer : BarebonesMessageBroker.Event
{
    public string Id { get; init; }

    public string EventName => "UserModule.UserCreated";

    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
}