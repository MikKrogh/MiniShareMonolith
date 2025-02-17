using MassTransit;

namespace PostsModule.Application.UserEvents
{
    public class UserCreatedEvent
    {
        public string UserName { get; init; }
        public Guid UserId { get; init; }
    }

    public class UserCreatedEventHandler : IConsumer<UserCreatedEvent>
    {
        public Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            throw new NotImplementedException();
        }
    }
}
