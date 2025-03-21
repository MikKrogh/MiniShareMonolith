namespace PostsModule.Application.UserEvents;

public class UserCreatedEvent(Guid userId, string userName)
{
    public string UserName { get; } = userName;
    public Guid UserId { get; } = userId;
}

