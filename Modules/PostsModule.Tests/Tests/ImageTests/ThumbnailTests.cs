
using PostsModule.Tests.Helper;
using System.Net;

namespace PostsModule.Tests.Tests.ImageTests;

public class ThumbnailTests : IClassFixture<PostsWebApplicationFactory>
{
    private readonly TestFacade testFacade;
    public ThumbnailTests(PostsWebApplicationFactory factory)
    {
        testFacade = new TestFacade(factory);
    }

    [Fact]

    public async Task GivenUserHasCreatedPost_WhenUserUploadsThumbnail_ThenSuccessIsReturned()
    {
        //Given 
        var user = await testFacade.SendCreateUserEvent();
        var createPostBody = PostRequestBuilder.GetValidDefaultBody();
        var createPostResponse = await testFacade.SendCreatePost(createPostBody, user.UserId);

        //When 
        var response = await testFacade.UploadThumbnail(createPostResponse.Result.PostId, createPostResponse.Result.Token, new byte[90]);

        //Then
        Assert.Equal(HttpStatusCode.OK, response);

    }

    [Fact]
    public async Task GivenUserHasCreatedPost_WhenUserUploadsThumbnail_ThenThumbnailIsSaved() 
    {
        //Given 
        var user = await testFacade.SendCreateUserEvent();
        var createPostBody = PostRequestBuilder.GetValidDefaultBody();
        var createPostResponse = await testFacade.SendCreatePost(createPostBody, user.UserId);
        var response = await testFacade.UploadThumbnail(createPostResponse.Result.PostId, createPostResponse.Result.Token, new byte[90]);

        //when
         var thumpnail = await testFacade.GetThumbnail(createPostResponse.Result.PostId);

        //Then
        Assert.Equal(HttpStatusCode.OK, response);
        Assert.NotNull(thumpnail);
        Assert.Equal(new byte[90], thumpnail.Result);
    }

    [Fact]
    public async Task GivenUserHasCreatedPostAndUploadedThumbnail_WhenUserUploadsAnotherThumbnail_ThenNewestThumbNailIsSaved()
    {
        //Given 
        var user = await testFacade.SendCreateUserEvent();
        var createPostBody = PostRequestBuilder.GetValidDefaultBody();
        var createPostResponse = await testFacade.SendCreatePost(createPostBody, user.UserId);
        var initialThumnailResponse = await testFacade.UploadThumbnail(createPostResponse.Result.PostId, createPostResponse.Result.Token, new byte[90]);

        //when
        var secondaryThumbnailResponse = await testFacade.UploadThumbnail(createPostResponse.Result.PostId, createPostResponse.Result.Token, new byte[10]);
        var thumpnail = await testFacade.GetThumbnail(createPostResponse.Result.PostId);

        //Then
        Assert.Equal(HttpStatusCode.OK, secondaryThumbnailResponse);
        Assert.NotNull(thumpnail);
        Assert.Equal(new byte[10], thumpnail.Result);


    }

    [Fact]
    public async Task GivenUserCreatedPostButWaitedTooLong_WhenUserUploadsThumpnail_ThenBadRequestIsReturned()
    {
        Assert.Fail();
    }

    [Fact]
    public async Task GivenUserCreatedPost_WhenUserUploadsFileAboveSizeLimit_ThenBadRequestIsReturned()
    {
        Assert.Fail();
    }

    [Fact]
    public async Task GivenUserCreatedPost_WhenUserUploadsFileWithInvalidExtension_ThenBadRequestIsReturned()
    {
        Assert.Fail();
    }
}
