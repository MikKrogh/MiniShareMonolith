using EngagementModule.Notification;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

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


    [Fact]
    public async Task GivenSomeoneLeavesSubCommentOnRootComment_WhenRootCommentatorChecksNotification_ThenPostWithRootCommentIsReturned()
    {
               // Given
        string postId = Guid.NewGuid().ToString();

        await _factory.PublishPostCreatedEvent(postId, Guid.NewGuid().ToString());

        string rootCommentatorId = Guid.NewGuid().ToString();
        var rootComment = new { Content = "This is a test comment." };
        var rootCommentResponse = await client.PostAsJsonAsync($"/Engagement/{postId}/comments?userId={rootCommentatorId}", rootComment);
        var responseContent = await rootCommentResponse.Content.ReadAsStringAsync();
        var rootCommentId = JsonSerializer.Deserialize<string>(responseContent);


        await client.PostAsync($"/Engagement/notifications?userId={rootCommentatorId}", null);

        string subCommentatorId = Guid.NewGuid().ToString();
        var subComment = new {
            Content = "This is a sub comment.",
            ParentCommentId = rootCommentId
        };
        await client.PostAsJsonAsync($"/Engagement/{postId}/comments?userId={subCommentatorId}", subComment);
        // When
        var notificationResponse = await client.GetFromJsonAsync<List<NotifocationsDto>>($"/Engagement/Notifications?userId={rootCommentatorId}");
        // Then
        Assert.NotNull(notificationResponse);
        Assert.Single(notificationResponse);
        Assert.Equal(postId, notificationResponse[0].PostId);
    }


    //it latest acitivtry is self, no notify
    [Fact]
    public async Task GivenUserSubmitsPost_WhenUserChecksForNotifcations_EmptyResultIsReturned()
    {
        // Given
        string postId = Guid.NewGuid().ToString();
        string creatorId = Guid.NewGuid().ToString();
        await _factory.PublishPostCreatedEvent(postId, creatorId);
        // When
        var notificationResponse = await client.GetFromJsonAsync<List<NotifocationsDto>>($"/Engagement/Notifications?userId={creatorId}");
        // Then
        Assert.NotNull(notificationResponse);
        Assert.Empty(notificationResponse);
    }

    [Fact]
    public async Task GivenUserSubmitsComment_WhenUserChecksForNotifcations_EmptyResultIsReturned()
    {
        string postId = Guid.NewGuid().ToString();
        string postCreatorId = Guid.NewGuid().ToString();
        await _factory.PublishPostCreatedEvent(postId, postCreatorId);

        string commentatorId = Guid.NewGuid().ToString();
        var comment = new { Content = "This is a test comment." };
        await client.PostAsJsonAsync($"/Engagement/{postId}/comments?userId={commentatorId}", comment);

        // When
        var notificationResponse = await client.GetFromJsonAsync<List<NotifocationsDto>>($"/Engagement/Notifications?userId={commentatorId}");

        // Then
        Assert.NotNull(notificationResponse);
        Assert.Empty(notificationResponse);
    }

    [Fact]
    public async Task GivenPostHasRootCommentAndUserLeftASubComment_WhenSubCommentatorChecksForNotifications_ThenEmptyResultIsReturned()
    {
        // Given
        string postId = Guid.NewGuid().ToString();
        string postCreatorId = Guid.NewGuid().ToString();
        await _factory.PublishPostCreatedEvent(postId, postCreatorId);

        string rootCommentatorId = Guid.NewGuid().ToString();
        var rootComment = new { Content = "This is a root comment." };
        var rootCommentResponse = await client.PostAsJsonAsync($"/Engagement/{postId}/comments?userId={rootCommentatorId}", rootComment);
        var responseContent = await rootCommentResponse.Content.ReadAsStringAsync();
        var rootCommentId = JsonSerializer.Deserialize<string>(responseContent);
        
        string subCommentatorId = Guid.NewGuid().ToString();
        var subComment = new {
            Content = "This is a sub comment.",
            ParentCommentId = rootCommentId
        };
        await client.PostAsJsonAsync($"/Engagement/{postId}/comments?userId={subCommentatorId}", subComment);
        // When
        var notificationResponse = await client.GetFromJsonAsync<List<NotifocationsDto>>($"/Engagement/Notifications?userId={subCommentatorId}");
        // Then
        Assert.NotNull(notificationResponse);
        Assert.Empty(notificationResponse);

    }

    [Fact]
    public async Task Test()
    {
        var postId = Guid.NewGuid().ToString();
        var postCreatorId = Guid.NewGuid().ToString();
        var subCommentatorId = Guid.NewGuid().ToString();

        await _factory.PublishPostCreatedEvent(postId, postCreatorId);
        var rootComment = new { Content = "This is a root comment." };
        var rootCommentResponse = await client.PostAsJsonAsync($"/Engagement/{postId}/comments?userId={postCreatorId}", rootComment);
        var json = await rootCommentResponse.Content.ReadAsStringAsync();
        var parentCommentId = JsonSerializer.Deserialize<string>(json);

        var subcomment = new
        {
            Content = "This is a sub comment.",
            ParentCommentId = parentCommentId
        };
        await client.PostAsJsonAsync($"/Engagement/{postId}/comments?userId={subCommentatorId}", subcomment);

        // When
        var notificationResponse = await client.GetFromJsonAsync<List<NotifocationsDto>>($"/Engagement/Notifications?userId={postCreatorId}");


        Assert.Single(notificationResponse);
    }
}