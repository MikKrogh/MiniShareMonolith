
using PostsModule.Tests.Helper;


namespace PostsModule.Tests.GetPostTests;
[Collection("Global Web Application Collection")]

public class GetPostTests : IClassFixture<PostsWebApplicationFactory>
{
    private readonly TestFacade testFacade;
    public GetPostTests(PostsWebApplicationFactory factory)
    {
        testFacade = new TestFacade(factory);
    }


    [Fact]
    internal async Task GivenPostExists_WhenUserAsksForPost_ThenCorrectValuesAreReturned()
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultBody();
        var create = await testFacade.SendCreatePost(createBody, user.UserId);

        // When
        var post = await testFacade.GetPost(create.Result.PostId);

        // Then
        Assert.True(post != null);
        Assert.False(string.IsNullOrEmpty(post.Id.ToString()));
        Assert.Equal(createBody.Title, post.Title);
        Assert.Equal(createBody.Description, post.Description);
        Assert.Equal(user.UserId, post.CreatorId);
        Assert.False(string.IsNullOrEmpty(post.CreatorName));
        Assert.Equal(createBody.PrimaryColor.ToLower(), post.PrimaryColor.ToString().ToLower());
        Assert.Equal(createBody.SecondaryColor.ToLower(), post.SecondaryColor.ToString().ToLower());
    }

}
