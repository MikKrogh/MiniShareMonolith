using MassTransit;
using PostsModule.Domain;
namespace PostsModule.Application.Create;

public sealed class CreatePostCommandConsumer : IConsumer<CreatePostCommand>
{
    private readonly IPostsRepository repository;

    public CreatePostCommandConsumer(IPostsRepository repository )
    {
        this.repository = repository;
    }
    public async Task Consume(ConsumeContext<CreatePostCommand> context)
    {
        if (IsValid(context.Message))
        {
            Post post = CreateDomainEntity(context.Message);
            await repository.Save(post);
            await context.RespondAsync(CommandResult<CreatePostCommandResult>.Success(new() { PostId = post.Id.ToString()}));
        }
        else
        {
            var result = CommandResult<CreatePostCommandResult>.FailedToValidate();
            await context.RespondAsync(result);
        }
    }

    private static Post CreateDomainEntity(CreatePostCommand context)
    {
        var post = Post.CreateNew(context.Title, context.CreatorId, context.FactionName);
        post.SetDescription(context.Description);
        post.SetPrimaryColor(context.PrimaryColor);
        post.SetSecondaryColor(context.SecondaryColor);
        return post;
    }

    private bool IsValid(CreatePostCommand command)
    {
        var validator = new CreatePostCommandValidator();
        var isValid = validator.IsValid(command);
        return isValid;
    }
}

public class CreatePostCommandValidator : Validator<CreatePostCommand>
{
    public bool isValid { get; init; } = false;
    public CreatePostCommandValidator()
    {
        MustNotBeEmptyGuid(x => x.CreatorId);
        MustNotContainsNumerics(x => x.Title);
        MustBeHigher(x => x.Title?.Length, 3);
        MustBeLower(x => x.Title?.Length, 33);
        MustBeHigher(x => x.FactionName?.Length, 3);
        MustBeLower(x => x.FactionName?.Length, 26);
        MustNotContainsNumerics(x => x.FactionName);
    }
}
public abstract class Validator<T> where T : class
{
    private List<Func<T, bool>> rules = new();
    //public void MustBeHigher(Func<T,int> target, int limit) => rules.Add(x => (target.Invoke(x)  > limit));
    public void MustBeHigher(Func<T, int?> target, int limit) => rules.Add(x =>
    {
        int? value = target.Invoke(x);
        return value.HasValue && value.Value > limit;

    });
    public void MustBeLower(Func<T, int?> target, int limit) => rules.Add(x =>
    {
        var value = target.Invoke(x);
        return value.HasValue && value.Value < limit;
    });
    public void MustNotContainsNumerics(Func<T, string?> target) => rules.Add(x =>
    {
        var value = target.Invoke(x);
        return !string.IsNullOrEmpty(value) && !value.Any(char.IsDigit);
    });
    public void MustNotBeEmptyGuid(Func<T, string> target) => rules.Add(x =>
    {
        string guidString = target.Invoke(x);
        return Guid.TryParse(guidString, out Guid newGuid) && newGuid != Guid.Empty;
    });

    public bool IsValid(T target)
    {
        return rules.All(x => x.Invoke(target));
    }

}
