
using PostsModule.Application;
using PostsModule.Tests.Helper;
using System.Net;

namespace PostsModule.Tests.Tests.GetPostsTests;

public class GetPostsTests: IClassFixture<PostsWebApplicationFactory>
{
    private readonly TestFacade testFacade;
    public GetPostsTests(PostsWebApplicationFactory factory)
    {
        testFacade = new TestFacade(factory);
    }

    
    [Fact]
    internal async Task GivenMultiplePostsExists_WhenUserAsksForPosts_ThenSuccessIsReturned()
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        var firstCreation = await testFacade.SendCreatePost(createBody);
        var secondCreation = await testFacade.SendCreatePost(createBody);
        
        // When        
        var posts = await testFacade.GetPosts();

        // Then
        Assert.Equal(HttpStatusCode.OK, posts.StatusCode);
    }
    //GivenMultiplePostsExists_WhenUserAsksForPosts_ThenCorrectValuesAreReturned
    [Fact]
    internal async Task GivenMultiplePostsExists_WhenUserAsksForPosts_ThenCorrectValuesAreReturned()
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();

        var firstPost = new PostRequestBuilder().Create(user.UserId)            
            .WithTitle("first")
            .WithFactionName("firstfaction")
            .WithDescription("firstdescp")
            .WithMainColor("red")
            .WithSecondaryColor("blue")
        .Build();

        var secondPost = new PostRequestBuilder().Create(user.UserId)
            .WithTitle("second")
            .WithFactionName("seoncdfaction")
            .WithDescription("seconddescp")
            .WithMainColor("yellow")
            .WithSecondaryColor("green")
        .Build();

        await testFacade.SendCreatePost(firstPost);
        await testFacade.SendCreatePost(secondPost);

        // When        
        var getPosts = await testFacade.GetPosts();

        // Then
        Assert.Equal(HttpStatusCode.OK, getPosts.StatusCode);
        Assert.NotEmpty(getPosts.Result);
        Assert.Equal(2, getPosts.Result.Count);
        Assert.True(RequestMatchesDto(getPosts.Result[0], firstPost));
        

    }

    private bool RequestMatchesDto(PostDto dto, PostRequest request)
    {
        return dto.Title == request.Title
            && dto.CreatorId == request.CreatorId
            && dto.FactionName == request.FactionName
            && dto.Description == request.Description
            && dto.PrimaryColor.ToString() == request.PrimaryColor
            && dto.SecondaryColor.ToString() == request.SecondaryColor;

    }

    //pagination

    //take

    //filter


    //allTheAbove
}
