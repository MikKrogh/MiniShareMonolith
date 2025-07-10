using BarebonesMessageBroker;
namespace UserModule.Features.CreateUser;

internal sealed class SignupCommandHandler 
{
    private readonly IUserRepository repository;
    private readonly IBus _bus;

    public SignupCommandHandler(IUserRepository repository,  IBus bus)
    {
        this.repository = repository;        
        this._bus = bus;
    }
    public async Task<SignupCommandResult> Handle(SignupCommand context)
    {

        if (!IsValidCommand(context))
        {
            return SignupCommandResult.BadRequest();            
        }
        try
        {
            var user = new User
            {
                UserName = context.DisplayName,
                Id = context.UserId,
                CreationDate = DateTime.UtcNow
            };
            await repository.CreateUser(user);
            await _bus.Publish(new UserCreatedEvent()
            {
                UserId = user.Id,
                UserName = user.UserName
            }, "UserModule.UserCreated");
            return SignupCommandResult.Success();
        }
        catch (Exception e)
        {
            return SignupCommandResult.InternalError();
        }
    }
    private bool IsValidCommand(SignupCommand command)
    {
        return !string.IsNullOrEmpty(command.UserId) && !string.IsNullOrEmpty(command.DisplayName);
    }
}
public class UserCreatedEvent
{
    public string UserId { get; set; }
    public string UserName { get; set; }
}