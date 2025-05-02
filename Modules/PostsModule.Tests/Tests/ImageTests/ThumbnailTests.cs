

using PostsModule.Tests.Helper;
using System.Net;
using PostsModule.Tests.ImageTests;

namespace PostsModule.Tests.ImageTests;

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
        //Given
        var user = await testFacade.SendCreateUserEvent();
        var createPostBody = PostRequestBuilder.GetValidDefaultBody();
        var createPostResponse = await testFacade.SendCreatePost(createPostBody, user.UserId);

        //When
        var token = testFacade.CreateToken(DateTime.UtcNow.AddMinutes(-20), createPostResponse.Result.PostId);
        var response = await testFacade.UploadThumbnail(createPostResponse.Result.PostId, token);

        //Then
        Assert.Equal(HttpStatusCode.InternalServerError, response);
        var getResponse = await testFacade.GetThumbnail(createPostResponse.Result.PostId);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GivenUserCreatedPost_WhenUserUploadsFileAboveSizeLimit_ThenBadRequestIsReturned()
    {
        var user = await testFacade.SendCreateUserEvent();
        var createPostBody = PostRequestBuilder.GetValidDefaultBody();
        var createPostResponse = await testFacade.SendCreatePost(createPostBody, user.UserId);

        //When 
        var response = await testFacade.UploadThumbnail(createPostResponse.Result.PostId, createPostResponse.Result.Token, new byte[510_000]);

        //Then
        Assert.Equal(HttpStatusCode.BadRequest, response);
    }

    [Theory]
    [MemberData(nameof(NonImageTypeFileExtensions))]
    public async Task GivenUserCreatedPost_WhenUserUploadsFileWithInvalidExtension_ThenBadRequestIsReturned(string? fileExtension)
    {
        var user = await testFacade.SendCreateUserEvent();
        var createPostBody = PostRequestBuilder.GetValidDefaultBody();
        var createPostResponse = await testFacade.SendCreatePost(createPostBody, user.UserId);
         
        //When 
        var response = await testFacade.UploadThumbnail(createPostResponse.Result.PostId, createPostResponse.Result.Token,fileExtension: fileExtension );

        //Then
        Assert.Equal(HttpStatusCode.BadRequest, response);
    }
    public static IEnumerable<object[]> NonImageTypeFileExtensions()
    {
        yield return new object[] { ".png" };
        yield return new object[] { ".jpg" };
        yield return new object[] { ".jpeg" };
        yield return new object[] { ".txt" };
        yield return new object[] { ".pdf" };
        yield return new object[] { ".doc" };
        yield return new object[] { ".docx" };
        yield return new object[] { ".xls" };
        yield return new object[] { ".xlsx" };
        yield return new object[] { ".ppt" };
        yield return new object[] { ".pptx" };
        yield return new object[] { ".zip" };
        yield return new object[] { ".rar" };
        yield return new object[] { ".7z" };
        yield return new object[] { ".tar" };
        yield return new object[] { ".gz" };
        yield return new object[] { ".bz2" };
        yield return new object[] { ".xz" };
        yield return new object[] { ".exe" };
        yield return new object[] { ".msi" };
        yield return new object[] { ".bat" };
        yield return new object[] { ".sh" };
        yield return new object[] { ".cmd" };
        yield return new object[] { ".ps1" };
        yield return new object[] { ".vbs" };
        yield return new object[] { ".js" };
        yield return new object[] { ".css" };
        yield return new object[] { ".html" };
        yield return new object[] { ".htm" };
        yield return new object[] { ".php" };
        yield return new object[] { ".asp" };
        yield return new object[] { ".aspx" };
        yield return new object[] { ".jsp" };
        yield return new object[] { ".cs" };
        yield return new object[] { ".java" };
        yield return new object[] { ".cpp" };
        yield return new object[] { ".c" };
        yield return new object[] { ".h" };
        yield return new object[] { ".hpp" };
        yield return new object[] { ".hxx" };
        yield return new object[] { ".cxx" };
        yield return new object[] { "" };
        yield return new object[] { null };
    }
}

