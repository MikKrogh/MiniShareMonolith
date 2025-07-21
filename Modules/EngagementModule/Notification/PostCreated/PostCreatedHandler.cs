using BarebonesMessageBroker;
using EngagementModule.Comments;
namespace EngagementModule.Notification.PostCreated;

internal class PostCreatedHandler : Listener<PostCreatedEvent>
{
    private readonly ChainActivityService chainService;
    public PostCreatedHandler(ChainActivityService chainService)
    {
        this.chainService = chainService;
    }


    public async Task Handle(PostCreatedEvent t)
    {
        var chain = new ActivityChain
        {
            Id = t.PostId,
            PostId = t.PostId,
            Chains = new List<ChainLink>(),
            DateChanged = DateTime.UtcNow
        };

        var chainlink = new ChainLink
        {
            UserId = t.CreatorId,
            AcitivtyChainId = chain.Id,
            Chain = chain
        };
        chain.Chains.Add(chainlink);
        await chainService.CreateChain(chain);
    }
}


internal sealed class PostCreatedEvent : Event
{
    public string Id {get; init;} = string.Empty;

    public string EventName => "PostModule.PostCreated";
    public DateTime Timestamp => throw new NotImplementedException();
    public string PostId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string CreatorId { get; init; } = string.Empty;
}
