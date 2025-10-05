using BarebonesMessageBroker;
using PostsModule.Domain;
namespace PostsModule.Application.Create;

public  class CreatePostCommandConsumer
{
    private readonly IPostsRepository repository;
    private readonly IBus bus;

    public CreatePostCommandConsumer(IPostsRepository repository, IBus bus)
    {
        this.repository = repository;
        this.bus = bus;
    }
    public async Task<CommandResult<CreatePostCommandResult>> Consume(CreatePostCommand context)
    {
        if (IsValid(context))
        {
            Post post = CreateDomainEntity(context);
            await repository.Save(post);
            await bus.Publish(new PostCreatedEvent
            {
                CreatorId = post.CreatorId,
                PostId = post.Id.ToString(),
                Title = post.Title
            }, "PostModule.PostCreated");
            return CommandResult<CreatePostCommandResult>.Success(new() { PostId = post.Id.ToString() });
        }
        else
        {
            var result = CommandResult<CreatePostCommandResult>.FailedToValidate();
            return result;
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
public class PostCreatedEvent
{
    public string CreatorId { get; set; }
    public string PostId { get; set; }
    public string Title { get; set; }    

}

public class CreatePostCommandValidator : Validator<CreatePostCommand>
{
    public bool isValid { get; init; } = false;
    public CreatePostCommandValidator()
    {
        MustNotBeEmptyString(x => x.CreatorId);
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
    public void MustNotBeEmptyString(Func<T, string> target) => rules.Add(x =>
    {
        string value = target.Invoke(x);
        return !string.IsNullOrEmpty(value);
    });

    public bool IsValid(T target)
    {
        return rules.All(x => x.Invoke(target));
    }

}
