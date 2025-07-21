using EngagementModule.Notification;
using System.Net;
using System.Net.Http.Json;

namespace EngagementModule.Tests;

public class NotificationTests : IClassFixture<EngagementWebApplication>
{
    HttpClient client;
    EngagementWebApplication _factory;

    public NotificationTests(EngagementWebApplication factory)
    {
        _factory = factory;
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GivenPostHasNewComment_WhenCreatorChecksForNotication_ThenNotificaitonExistsForPost()
    {
        // Given
        string postId = Guid.NewGuid().ToString();
        string creatorId = Guid.NewGuid().ToString();
        await _factory.PublishPostCreatedEvent(postId, creatorId);
        await client.PostAsync($"/Engagement/notifications?userId={creatorId}", null);

        string commentatorId = Guid.NewGuid().ToString();
        var comment = new { Content = "This is a test comment." };
        await client.PostAsJsonAsync($"/Engagement/{postId}/comments?userId={commentatorId}", comment);

        //When
        var notificationResponse = await client.GetFromJsonAsync<List<NotifocationsDto>>($"/Engagement/Notifications?userId={creatorId}");
        // Then
        Assert.NotNull(notificationResponse);
        Assert.Single(notificationResponse);
        Assert.Equal(postId, notificationResponse[0].PostId);
    }

    [Fact]
    public async Task GivenPostExistsWithCommentAndCreatorHasSyncedAfterCommentCreation_WhenUserChecksForNoticiation_ThenEmptySuccessIsReturned()
    {
        string postId = Guid.NewGuid().ToString();
        string creatorId = Guid.NewGuid().ToString();
        await _factory.PublishPostCreatedEvent(postId, creatorId);
        await client.PostAsync($"/Engagement/notifications?userId={creatorId}", null);

        string commentatorId = Guid.NewGuid().ToString();
        var comment = new { Content = "This is a test comment." };
        await client.PostAsJsonAsync($"/Engagement/{postId}/comments?userId={commentatorId}", comment);
        var firstFetch = await client.GetFromJsonAsync<List<NotifocationsDto>>($"/Engagement/Notifications?userId={creatorId}");

        await client.PostAsync($"/Engagement/notifications?userId={creatorId}", null);
        //When
        var secondFetch = await client.GetFromJsonAsync<List<NotifocationsDto>>($"/Engagement/Notifications?userId={creatorId}");
        // Then
        Assert.NotNull(firstFetch);
        Assert.Single(firstFetch);
        Assert.NotNull(secondFetch);
        Assert.Empty(secondFetch);

    }

}