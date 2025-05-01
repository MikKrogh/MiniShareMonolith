
using PostsModule.Tests.Helper;
using System.Net;

namespace PostsModule.Tests.ImageTests;

public class ImageTests : IClassFixture<PostsWebApplicationFactory>
{
    private readonly TestFacade testFacade;
    public ImageTests(PostsWebApplicationFactory factory)
    {
        testFacade = new TestFacade(factory);
    }

    [Fact]
    public async Task GivenUserHasCreatedNewPost_WhenUserAddsImageToPost_ThenSuccessIsReturned()
    {
        //Given 
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultBody();
        var create = await testFacade.SendCreatePost(createBody, user.UserId);

        //When
        var response = await testFacade.UploadImage(create.Result.PostId, create.Result.Token);

        //Then
        Assert.True(response == HttpStatusCode.OK);
    }

    [Fact]
    public async Task GivenUserHasCreatedNewPost_WhenUserAddsImageToPost_ThenPostContainsImagePath()
    {
        //Given 
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultBody();
        var create = await testFacade.SendCreatePost(createBody, user.UserId);

        //When
        var response = await testFacade.UploadImage(create.Result.PostId, create.Result.Token);

        //Then
        var getResponse = await testFacade.GetPost(create.Result.PostId);

        Assert.NotNull(getResponse);
        Assert.NotEmpty(getResponse.Images);
        Assert.Single(getResponse.Images);
    }

    [Fact]
    public async Task GivenUserHasCreatedPost_WhenUserUploadsImages_ThenImagesAreStoredInBlob()
    {
        //Given 
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultBody();
        var create = await testFacade.SendCreatePost(createBody, user.UserId);

        //When
        await testFacade.UploadImage(create.Result.PostId, create.Result.Token);
        await testFacade.UploadImage(create.Result.PostId, create.Result.Token);

        //Then
        var getResponse = await testFacade.GetPost(create.Result.PostId);
        var images1 = await testFacade.GetImage(create.Result.PostId, getResponse.Images.First());
        var images2 = await testFacade.GetImage(create.Result.PostId, getResponse.Images.Last());

        Assert.NotNull(images1.Result);
        Assert.NotNull(images2.Result);

        Assert.NotEmpty(images1.Result);
        Assert.NotEmpty(images2.Result);


    }

    [Fact]
    public async Task GivenUserExistsAndHasCreatedAPost_WhenUserUploadsImageBiggerThan9Mb_ThenBadRequrstIsReturned()
    {
        //Given 
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultBody();
        var create = await testFacade.SendCreatePost(createBody, user.UserId);
        //When
        var response = await testFacade.UploadImage(create.Result.PostId, create.Result.Token, new byte[9_000_001]);
        //Then
        Assert.True(response == HttpStatusCode.BadRequest, $"request statuscode was: {response}");
    }


    [Theory]
    [MemberData(nameof(NonImageTypeFileExtensions))]
    public async Task GivenUserExistsAndHasCreatedAPost_WhenUserUploadsImageWithWrongExtension_ThenBadRequestIsReturned(string fileExtension)
    {
        //Given 
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultBody();
        var create = await testFacade.SendCreatePost(createBody, user.UserId);

        //When
        var response = await testFacade.UploadImage(create.Result.PostId, create.Result.Token, fileExtension: fileExtension);

        //Then
        Assert.True(response == HttpStatusCode.BadRequest, $"request statuscode was: {response}");
    }

    [Fact]
    public async Task GivenUserHasCreatedPostAndAddedEightImages_WhenUserUploadsAnotherImage_ThenBadRequestIsReturnedAndPostOnlyHasEightImages()
    {
        // Given 
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultBody();
        var create = await testFacade.SendCreatePost(createBody, user.UserId);

        var uploadTasks = Enumerable.Range(0, 8)
            .Select(x => testFacade.UploadImage(create.Result.PostId, create.Result.Token));
        await Task.WhenAll(uploadTasks);

        var getdResponse = await testFacade.GetPost(create.Result.PostId);


        // When
        var response = await testFacade.UploadImage(create.Result.PostId, create.Result.Token);

        // Then        
        var getResponse = await testFacade.GetPost(create.Result.PostId);
        Assert.NotNull(getResponse);
        Assert.NotEmpty(getResponse.Images);
        Assert.Equal(8, getResponse.Images.Count());
    }

    [Fact]
    public async Task GivenUserHasCreatedNewPost_WhenNonValidTokenIsUsed_ThenInternalErrorIsReturnedAndNoImageIsAddedToPost()
    {
        //Given 
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultBody();
        var create = await testFacade.SendCreatePost(createBody, user.UserId);

        //When        
        var response = await testFacade.UploadImage(create.Result.PostId, "nonValidToken");

        //Then
        Assert.True(response == HttpStatusCode.InternalServerError);
        var getResponse = await testFacade.GetPost(create.Result.PostId);
        Assert.NotNull(getResponse);
        Assert.Empty(getResponse.Images);
    }

    [Fact]
    public async Task GivenUserHasCreatedNewPost_WhenTamperedTokenIsUsed_ThenInternalErrorIsReturnedAndNoImageIsAddedToPost()
    {
        //Given 
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultBody();
        var create = await testFacade.SendCreatePost(createBody, user.UserId);

        //When        
        var response = await testFacade.UploadImage(create.Result.PostId, create.Result.Token + "tampered");

        //Then
        Assert.True(response == HttpStatusCode.InternalServerError);
        var getResponse = await testFacade.GetPost(create.Result.PostId);
        Assert.NotNull(getResponse);
        Assert.Empty(getResponse.Images);
    }

    [Fact]
    public async Task GivenUserHasCreatedPost_WhenExpiredTokenIsUsed_ThenInternalErrorIsReturnedAndNoImageIsAddedToPost()
    {
        //Given 
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultBody();
        var create = await testFacade.SendCreatePost(createBody, user.UserId);
        //When        
        var token = testFacade.CreateToken(DateTime.UtcNow.AddDays(-1), create.Result.PostId);
        var response = await testFacade.UploadImage(create.Result.PostId, token);
        //Then
        Assert.True(response == HttpStatusCode.InternalServerError);
        var getResponse = await testFacade.GetPost(create.Result.PostId);
        Assert.NotNull(getResponse);
        Assert.Empty(getResponse.Images);
    }

    [Fact]
    public async Task GivenUserHasCreatedPost_WhenTokenIsValidForDifferentPost_ThenInternalErrorIsReturnedAndNoImageIsAddedToPost()
    {
        //Given 
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultBody();
        var create = await testFacade.SendCreatePost(createBody, user.UserId);
        var create2 = await testFacade.SendCreatePost(createBody, user.UserId);

        //When        
        var response = await testFacade.UploadImage(create.Result.PostId, create2.Result.Token);
        //Then
        Assert.True(response == HttpStatusCode.InternalServerError);
        var getResponse = await testFacade.GetPost(create.Result.PostId);
        var getResponse2 = await testFacade.GetPost(create2.Result.PostId);
        Assert.NotNull(getResponse2);
        Assert.NotNull(getResponse);
        Assert.Empty(getResponse.Images);
        Assert.Empty(getResponse2.Images);
    }


    [Fact]
    public async Task GivenUserHasCreatedPostAndUploadedOneImage_WhenUserFetchesImage_ThenResponseIsSuccessImageIsReturned()
    {
        //Given 
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultBody();
        var create = await testFacade.SendCreatePost(createBody, user.UserId);
        await testFacade.UploadImage(create.Result.PostId, create.Result.Token);

        //When
        var getResponse = await testFacade.GetPost(create.Result.PostId);
        string imageId = getResponse.Images.SingleOrDefault();
        var response = await testFacade.GetImage(create.Result.PostId, imageId);

        //Then
        Assert.True(response.StatusCode == HttpStatusCode.OK);
        Assert.NotNull(response.Result);
        Assert.NotEmpty(response.Result);
    }

    [Fact]
    public async Task GivenUserUploadedSpecificFile_WhenUserFetchesImage_ThenSpecificFileIsReturned()
    {
        //Given 
        byte[] file = new byte[200];
        var user = await testFacade.SendCreateUserEvent();
        var create = await testFacade.SendCreatePost(PostRequestBuilder.GetValidDefaultBody(), user.UserId);
        await testFacade.UploadImage(create.Result.PostId, create.Result.Token, file, fileExtension: ".png");

        //When
        var getResponse = await testFacade.GetPost(create.Result.PostId);
        string? imageId = getResponse.Images.SingleOrDefault();
        var response = await testFacade.GetImage(create.Result.PostId, imageId);

        //Then
        Assert.True(response.StatusCode == HttpStatusCode.OK);
        Assert.NotNull(response.Result);
        Assert.NotEmpty(response.Result);
        Assert.Equal(file, response.Result);
    }

    [Fact]
    public async Task GivenUserHasCreatedPostAndUploadedOneImage_WhenUserFetchesImageWithWrongImageId_ThenNotFoundIsReturned()
    {
        //Given
        var user = await testFacade.SendCreateUserEvent();
        var create = await testFacade.SendCreatePost(PostRequestBuilder.GetValidDefaultBody(), user.UserId);
        await testFacade.UploadImage(create.Result.PostId, create.Result.Token);

        //When
        var response = await testFacade.GetImage(create.Result.PostId, "wrongImageId");

        //Then
        Assert.True(response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GivenUserHasCreatedPostAndUploadedOneImage_WhenUserFetchesImageWithWrongPostId_ThenNotFoundIsReturned()
    {
        //Given
        var user = await testFacade.SendCreateUserEvent();
        var create = await testFacade.SendCreatePost(PostRequestBuilder.GetValidDefaultBody(), user.UserId);
        await testFacade.UploadImage(create.Result.PostId, create.Result.Token);

        //When
        var getResponse = await testFacade.GetPost(create.Result.PostId);
        var response = await testFacade.GetImage("wrongPostId", getResponse.Images.SingleOrDefault());

        //Then
        Assert.True(response.StatusCode == HttpStatusCode.NotFound);
    }

    public static IEnumerable<object[]> NonImageTypeFileExtensions()
    {
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
    }
}
