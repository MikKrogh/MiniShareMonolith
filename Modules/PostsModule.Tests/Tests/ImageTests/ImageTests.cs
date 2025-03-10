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
        Assert.True(false);

    }
}
