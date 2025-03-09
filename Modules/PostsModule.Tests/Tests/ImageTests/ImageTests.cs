using Microsoft.Extensions.DependencyInjection;
using PostsModule.Application.UserEvents;
using PostsModule.Presentation.Endpoints;
using PostsModule.Tests.Helper;
using System.Net;
using System.Net.Http.Json;


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
}
