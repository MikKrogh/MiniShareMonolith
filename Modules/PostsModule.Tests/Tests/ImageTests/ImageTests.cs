using PostsModule.Presentation.Endpoints;
using PostsModule.Tests.Helper;
using System.Net;

namespace PostsModule.Tests.Tests.ImageTests;

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
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        var create = await testFacade.SendCreatePost(createBody);


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
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        var create = await testFacade.SendCreatePost(createBody);

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
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        var create = await testFacade.SendCreatePost(createBody);

        //When
        await testFacade.UploadImage(create.Result.PostId, create.Result.Token);
        await testFacade.UploadImage(create.Result.PostId, create.Result.Token);

        //Then
        var filesInBlob = testFacade.FilesInDirecory(create.Result.PostId);
        Assert.Equal(2, filesInBlob.Count());
    }

    [Fact]
    public async Task GivenUserExistsAndHasCreatedAPost_WhenUserUploadsImageBiggerThan9Mb_ThenBadRequrstIsReturned()
    {
        //Given 
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        var create = await testFacade.SendCreatePost(createBody);
        //When
        var response = await testFacade.UploadImage(create.Result.PostId, create.Result.Token, 9_000_001);
        //Then
        Assert.True(response == HttpStatusCode.BadRequest, $"request statuscode was: {response}");
    }


    [Theory]
    [MemberData(nameof(FileExtensions))]
    public async Task GivenUserExistsAndHasCreatedAPost_WhenUserUploadsImageWithWrongExtension_ThenBadRequestIsReturned(string fileExtension)
    {
        //Given 
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        var create = await testFacade.SendCreatePost(createBody);

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
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        var create = await testFacade.SendCreatePost(createBody);

        var uploadTasks = Enumerable.Range(0, 8)
            .Select(x => testFacade.UploadImage(create.Result.PostId, create.Result.Token));
        await Task.WhenAll(uploadTasks);

        var getdResponse = await testFacade.GetPost(create.Result.PostId);


        // When
        var response = await testFacade.UploadImage(create.Result.PostId, create.Result.Token);

        // Then
        var t = StreamBank.StreamsByPost;
        var getResponse = await testFacade.GetPost(create.Result.PostId);
        Assert.NotNull(getResponse);
        Assert.NotEmpty(getResponse.Images);
        Assert.Equal(8, getResponse.Images.Count());
    }




    //tokenValidation
    //PostExists


    public static IEnumerable<object[]> FileExtensions()
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
