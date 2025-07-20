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
    public async Task WhenPostHasTwoComments_WhenUserGetsComments_ThenCorrectCommentAreReturned()
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

    [Fact]
    public async Task GivenCommentExists_WhenUserCreatesSubComment_ThenSuccessIsReturned()
    {
               // Given
        string postId = Guid.NewGuid().ToString();
        string userId = Guid.NewGuid().ToString();
        var comment = new { Content = "This is a root comment." };
        await client.PostAsJsonAsync($"/Engagement/{postId}/comments?userId={userId}", comment);
        var getCommentsRespone = await client.GetAsync($"/Engagement/{postId}/comments");
        var comments = await getCommentsRespone.Content.ReadFromJsonAsync<List<CommentDto>>();


        // When
        var subComment = new 
        { 
            Content = "This is a sub-comment.",
            ParentCommentId = comments.First().CommentId
        };
        var response = await client.PostAsJsonAsync($"/Engagement/{postId}/comments?userId={userId}", subComment);

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GivenPostExistsWithRootAndSubComments_WhenUserGetComments_ThenResultContainsAllComments()
    {
        // Given
        string postId = Guid.NewGuid().ToString();

        string rootCommentatorId = Guid.NewGuid().ToString();
        var rootComment = new { Content = "This is a root comment." };
        await client.PostAsJsonAsync($"/Engagement/{postId}/comments?userId={rootCommentatorId}", rootComment);
        var getCommentsResponse = await client.GetAsync($"/Engagement/{postId}/comments");
        var comments = await getCommentsResponse.Content.ReadFromJsonAsync<List<CommentDto>>();

        string subCommentatorOneId = Guid.NewGuid().ToString();
        var subCommentOne = new
        {
            Content = "This is  sub-commentOne.",
            ParentCommentId = comments.Single().CommentId
        };
        await client.PostAsJsonAsync($"/Engagement/{postId}/comments?userId={subCommentatorOneId}", subCommentOne);

        string subCommentatorTwoId = Guid.NewGuid().ToString();
        var subCommentTwo = new
        {
            Content = "This is  sub-commentTwo.",
            ParentCommentId = comments.Single().CommentId
        };
        await client.PostAsJsonAsync($"/Engagement/{postId}/comments?userId={subCommentatorTwoId}", subCommentTwo);

        // When
        var response = await client.GetAsync($"/Engagement/{postId}/comments");
        var result = await response.Content.ReadFromJsonAsync<List<CommentDto>>();

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, c => c.Content == rootComment.Content && c.UserId == rootCommentatorId);
        Assert.Contains(result, c => c.Content == subCommentOne.Content && c.UserId == subCommentatorOneId);
        Assert.Contains(result, c => c.Content == subCommentTwo.Content && c.UserId == subCommentatorTwoId);
    }
}

