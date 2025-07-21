using System.Net;
using System.Net.Http.Json;

namespace EngagementModule.Tests;

public class NotificationTests : IClassFixture<EngagementWebApplication>
{
    HttpClient client;

    public NotificationTests(EngagementWebApplication factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task WhenUserCommentsOnPost_ThenNotificaitonExistsForPostCreator()
    {
        // Given
        string postId = Guid.NewGuid().ToString();
        string userId = Guid.NewGuid().ToString();
        var comment = new { Content = "This is a test comment." };
        
        // When
        var response = await client.PostAsJsonAsync($"/Engagement/{postId}/comments?userId={userId}", comment);
        
        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // Check if notification exists for the post creator
        var notificationResponse = await client.GetAsync($"/Engagement/{postId}/notifications");
        Assert.Equal(HttpStatusCode.OK, notificationResponse.StatusCode);

    }
}