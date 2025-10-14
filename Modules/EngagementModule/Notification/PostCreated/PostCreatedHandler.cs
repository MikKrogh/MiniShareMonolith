using BarebonesMessageBroker;
using EngagementModule.Comments;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
namespace EngagementModule.Notification.PostCreated;

internal class PostCreatedHandler : Listener<PostCreatedEvent>
{
    private readonly ChainActivityService chainService;
    public PostCreatedHandler(ChainActivityService chainService)
    {
        this.chainService = chainService;
    }

    public static async Task<IResult> Process([FromServices]ChainActivityService chainService, [FromQuery]string postId, string creatorId)
    {
        var t = new PostCreatedEvent
        {
            PostId = postId,
            CreatorId = creatorId,
        };
        var chain = new ActivityChain
        {
            Id = t.PostId,
            PostId = t.PostId,
            Chains = new List<ChainLink>(),
            DateChanged = default
        };

        var chainlink = new ChainLink
        {
            UserId = t.CreatorId,
            AcitivtyChainId = chain.Id,
            Chain = chain
        };
        chain.Chains.Add(chainlink);
        await chainService.CreateChain(chain);
        return Results.Ok();

    }

    public async Task Handle(PostCreatedEvent t)
    {
        var chain = new ActivityChain
        {
            Id = t.PostId,
            PostId = t.PostId,
            Chains = new List<ChainLink>(),
            DateChanged = default
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
