
using PostsModule.Tests.Helper;


namespace PostsModule.Tests.GetPostTests;
[Collection("Global Web Application Collection")]

public class GetPostTests: IClassFixture<PostsWebApplicationFactory>
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
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        var create = await testFacade.SendCreatePost(createBody);

        // When
        var post = await testFacade.GetPost(create.Result.PostId);

        // Then
        Assert.True(post != null);
        Assert.False(string.IsNullOrEmpty(post.Id.ToString()));
        Assert.Equal(createBody.Title, post.Title);
        Assert.Equal(createBody.Description, post.Description);
        Assert.Equal(createBody.CreatorId, post.CreatorId);
        Assert.False(string.IsNullOrEmpty(post.CreatorName));
        Assert.Equal(createBody.PrimaryColor.ToLower(), post.PrimaryColor.ToString().ToLower());
        Assert.Equal(createBody.SecondaryColor.ToLower(), post.SecondaryColor.ToString().ToLower());
    }

    [Fact]
    public async Task GivenPostExistsWithTwoImages_WhenUserAsksForPost_thenResponseContainsTwoImagePaths()
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        var create = await testFacade.SendCreatePost(createBody);

        await testFacade.UploadImage(create.Result.PostId, create.Result.Token);
        await testFacade.UploadImage(create.Result.PostId, create.Result.Token);

        // When
        var post = await testFacade.GetPost(create.Result.PostId);

        // Then
        Assert.True(post != null);
        Assert.NotEmpty(post.Images);
        Assert.Equal(2, post.Images.Count());
    }
}
