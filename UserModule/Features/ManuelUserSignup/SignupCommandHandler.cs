using MassTransit;

namespace UserModule.Features.ManuelUserSignup;

public sealed class SignupCommandHandler : IConsumer<SignupCommand>
{
    private readonly IUserIdentityProvider identityProvider;

    public SignupCommandHandler(IUserIdentityProvider identityProvider)
    {
        this.identityProvider = identityProvider;
    }

    public async Task Consume(ConsumeContext<SignupCommand> context)
    {
        try
        {
            var createdUser = await identityProvider.CreateUserIdentity(context.Message.Email, context.Message.Password);
            await context.RespondAsync(SignupCommandResult.Success());
            //await context.RespondAsync(new SignupCommandResult());

        }
        catch (Exception)
        {
            await context.RespondAsync(SignupCommandResult.InternalError());
        }
    }
}
