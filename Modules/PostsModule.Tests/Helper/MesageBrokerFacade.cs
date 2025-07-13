
using BarebonesMessageBroker;

namespace PostsModule.Tests.Helper;

public class MesageBrokerFacade
{
    private readonly TestBus _bus;

    public MesageBrokerFacade(IBus bus)
    {
        _bus = bus as TestBus ?? throw new Exception("couldnt find Testbus");

    }
    public async Task<UserCreatedEvent> SendUserCreatedEvent(Guid? userId = null, string? username = null)
    {
        var userCreateEvent = new UserCreatedEvent()
        {
            UserId = userId.ToString() ?? Guid.NewGuid().ToString(),
            UserName = username ?? "some random Name"
        };
        await _bus.Publish(userCreateEvent, "UserModule.UserCreated");
        return userCreateEvent;
    }
    public async Task Publish<T>(T message, string eventName) where T : class => await _bus.Publish(message, eventName);

    public bool AssertExactlyOneMessageMatch<MessageType>(Predicate<MessageType> predicate, string eventName)
    {
        var publishedEvents = _bus.PublishedEvents.Where(x => x.EventName == eventName);
        var matchingEvents = publishedEvents.Where(x => x.Message is MessageType message && predicate.Invoke(message)).ToList();
        return matchingEvents?.Count == 1;
    }
}
