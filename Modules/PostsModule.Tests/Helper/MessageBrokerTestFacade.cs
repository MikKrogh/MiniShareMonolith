using MassTransit;
using MassTransit.Testing;
using PostsModule.Application.UserEvents;

namespace PostsModule.Tests.Helper;

public class MessageBrokerTestFacade
{
    private readonly IBus _bus;
    private readonly ITestHarness _harness;

    public MessageBrokerTestFacade(IBus bus, ITestHarness harnes)
    {
        _bus = bus;
        _harness = harnes;
    }
    public async Task<UserCreatedEvent> SendUserCreatedEvent(Guid? userId = null, string? username = null)
    {
        var userCreateEvent = new UserCreatedEvent(userId ?? Guid.NewGuid(), username ?? "some random Name");
        await _bus.Publish(userCreateEvent);
        return userCreateEvent;
    }
    public async Task Publish<T>(T message) where T : class => await _bus.Publish(message);

    public async Task<bool> AssertExactlyOneMessageMatch<MessageType>(Predicate<MessageType> predicate) where MessageType : class
    {
        FilterDelegate<IPublishedMessage<MessageType>> filter = (msg) => msg.MessageType == typeof(MessageType);
        var messagesMatchingMessageType = _harness.Published.Select(filter);

        int foundMatches = 0;
        foreach (var message in messagesMatchingMessageType)
        {
            bool hasFailures = predicate.Invoke(message.MessageObject as MessageType);
            if (!hasFailures)
                foundMatches++;

        }

        return foundMatches == 1 ? true : false;
    }

    public async Task WaitUntillEventHasBeenConsumed<T>(Predicate<T>? predicate = null) where T : class
    {
        var t = _harness.Consumed.SelectAsync<T>();
        await foreach (var message in t)
        {
            if (message.MessageType == typeof(T))
            {
                if (predicate is not null)
                {
                    var isValid = predicate.Invoke(message.MessageObject as T);
                    if (isValid)
                        return;
                    continue;
                }
                return;
            }
        }
    }
}
