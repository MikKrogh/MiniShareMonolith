namespace EventMessages;
public record UserCreatedEvent
{
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
}
