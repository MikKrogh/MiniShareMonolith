using BarebonesMessageBroker;
using EventMessages;
using MassTransit;
using PostsModule.Domain;

namespace PostsModule.Application.UserEvents;

public sealed class UserCreatedEventHandler : IConsumer<UserCreatedEvent>, Listener<TmpEventUSer>

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

    public Task Handle(TmpEventUSer t)
    {
        Console.WriteLine("hello createdevent {0} {1}", t.UserName, t.UserId);
        return Task.CompletedTask;
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