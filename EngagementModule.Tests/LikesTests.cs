using System.Net;
using System.Net.Http.Json;
namespace EngagementModule.Tests;

public class LikesTests : IClassFixture<EngagementWebApplication>
{
    HttpClient client;

    public LikesTests(EngagementWebApplication factory)
    {
        client = factory.CreateClient();
    }


    [Fact]
    public async Task WhenUserLikesPost_ThenSuccessIsReturned()
    {
        // When
        string postId = GetGuid();
        string userId = GetGuid();
        var response = await client.PostAsync($"/Engagement/{postId}/likes?userId={userId}",null);

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GivenUserLikedPost_WhenUserUnlikesPost_ThenSuccesIsReturned()
    {
        // Given
        string postId = GetGuid();
        string userId = GetGuid(); 
        var t = await client.PostAsync($"/Engagement/{postId}/Likes?userId={userId}", null);

        // When
        var response = await client.DeleteAsync($"/Engagement/{postId}/Likes?userId={userId}");
        
        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GivenPostHasTwoLikes_WhenAmountOfLikesIsRequested_ThenTwoIsReturned()
    {
        // Given
        var postId = GetGuid();
        var userId1 = GetGuid();
        var userId2 = GetGuid();

        await client.PostAsync($"/Engagement/{postId}/Likes?userId={userId1}", null);
        await client.PostAsync($"/Engagement/{postId}/Likes?userId={userId2}", null);

        //When
        var seralizedResponse = await client.GetFromJsonAsync<LikesResponseObject>($"/Engagement/{postId}/Likes/Count");

        //Then
        Assert.Equal(2, seralizedResponse?.Count);   
    }

    [Fact]
    public async Task GivenUserLikedPost_WhenUserLikesPostAgain_ThenSuccessIsReturnedAndPostsTotalLikesIsOne()
    {
        //Given
        var postId = GetGuid();
        var userId = GetGuid();
        await client.PostAsync($"/Engagement/{postId}/Likes?userId={userId}", null);

        //When
        var response = await client.PostAsync($"/Engagement/{postId}/Likes?userId={userId}", null);

        //Then
        var seralizedResponse = await client.GetFromJsonAsync<LikesResponseObject>($"/Engagement/{postId}/Likes/Count");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, seralizedResponse?.Count);
    }

    [Fact]
    public async Task WhenUserGetsCountForNonexistingPost_ThenCountOfZeroIsreturnedFoundIsReturned()
    {
        // Given
        var postId = GetGuid();
        // When
        var seralizedResponse = await client.GetFromJsonAsync<LikesResponseObject>($"/Engagement/{postId}/Likes/Count");

        // then        
        Assert.Equal(0, seralizedResponse?.Count);
    }


    [Fact]
    public async Task GivenPostHasZeroLikes_WhenUserUnlikesPost_ThenPostStillHasZeroLikesAndSuccessIsReturned()
    {
        var postId = GetGuid();
        var userId = GetGuid();
        // When
        var response = await client.DeleteAsync($"/Engagement/{postId}/Likes?userId={userId}");
        // Then
        var count = await client.GetFromJsonAsync<LikesResponseObject>($"/Engagement/{postId}/Likes/Count");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(0, count?.Count);






    }



    private string GetGuid() => Guid.NewGuid().ToString();
    private class LikesResponseObject
    {
        public int Count { get; set; }
    }
}
