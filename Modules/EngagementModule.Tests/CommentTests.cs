using EngagementModule.Comments;
using System.Net;
using System.Net.Http.Json;

namespace EngagementModule.Tests;

public class CommentTests : IClassFixture<EngagementWebApplication>
{
    HttpClient client;
    public CommentTests(EngagementWebApplication factory)
    {
        client = factory.CreateClient();
    }


    [Fact]
    public async Task WhenUserCommentsOnPost_ThenSuccessIsReturned()
    {
        // When
        string postId = Guid.NewGuid().ToString();
        string userId = Guid.NewGuid().ToString();
        var comment = new { Content = "This is a test comment." };
        var response = await client.PostAsJsonAsync($"/Engagement/{postId}/comments?userId={userId}", comment);
        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    [Fact]
    public async Task WhenPostHasTwoComments_WhenUserGetsComments_ThenResultsIsCorrect()
    {
               // Given
        string postId = Guid.NewGuid().ToString();
        string userId = Guid.NewGuid().ToString();
        var comment1 = new { Content = "First comment." };
        var comment2 = new { Content = "Second comment." };
        
        await client.PostAsJsonAsync($"/Engagement/{postId}/comments?userId={userId}", comment1);
        await client.PostAsJsonAsync($"/Engagement/{postId}/comments?userId={userId}", comment2);
        // When
        var response = await client.GetAsync($"/Engagement/{postId}/comments");
        
        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var comments = await response.Content.ReadFromJsonAsync<List<CommentDto>>();
        Assert.NotNull(comments);
        Assert.Equal(2, comments.Count);
        Assert.Contains(comments, c => c.Content == "First comment.");
        Assert.Contains(comments, c => c.Content == "Second comment.");
    }
}

