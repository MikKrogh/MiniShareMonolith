using MassTransit;
using MassTransit.Testing;

namespace PostsModule.Tests;

public class MessageBrokerTestFacade
{
    private readonly IBus _bus;
    private readonly ITestHarness _harness;

    public MessageBrokerTestFacade(IBus bus, ITestHarness harnes)
    {
        _bus = bus;
        _harness = harnes;
    }

    public async Task Publish<T>(T message) where T : class => await _bus.Publish(message);

    public async Task AssertExactlyOneMessageMatch<MessageType>(Predicate<MessageType> predicate) where MessageType : class
    {
        FilterDelegate<IPublishedMessage<MessageType>> filter = (msg) => msg.MessageType == typeof(MessageType);
        var messagesMatchingMessageType = _harness.Published.SelectAsync(filter);

        int foundMatches = 0;
        await foreach (var message in messagesMatchingMessageType)
        {
            bool hasFailures = predicate.Invoke(message.MessageObject as MessageType);
            if (!hasFailures) 
                foundMatches++;
            
        }
        int one = 1;
        Assert.Equal(foundMatches, one);
    }
}
