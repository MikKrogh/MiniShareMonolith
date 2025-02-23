using Microsoft.Extensions.DependencyInjection;
using PostsModule.Application.UserEvents;
using PostsModule.Presentation.Endpoints;
using PostsModule.Tests.CreatePostTests;
using PostsModule.Tests.Helper;
using System.Net.Http.Json;

namespace PostsModule.Tests.GetPostTests;

public class GetPostTests : IClassFixture<PostsWebApplicationFactory>
{
    private readonly PostsWebApplicationFactory _factory;
    private readonly HttpClient _client;
    public GetPostTests(PostsWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    //GivenPostExists_WhenUserAsksForThePost_ThenResponseIs200
    [Fact]
    public async Task GivenPostExists_WhenUserAsksForThePost_ThenResponseIs200()
    {
        // Given
        //Given         
        var existingUser = await _factory.MessageBrokerTestFacade.SendUserCreatedEvent(Guid.NewGuid(), "John Does");
        await _factory.MessageBrokerTestFacade.WaitUntillEventHasBeenConsumed<UserCreatedEvent>(x => x.UserId == existingUser.UserId);

        var createBody = PostTestHelper.GetValidDefaultRequest();
        createBody.CreatorId = existingUser.UserId.ToString();
        var response = await _client.PostAsJsonAsync("/Posts", createBody);
        var responseContent = await response.Content.ReadFromJsonAsync<CreatePostResponse>();

        // When
        var getResponse = await _client.GetAsync($"/Posts/{responseContent.PostId}");

        // Then
        Assert.True(getResponse.IsSuccessStatusCode);
    }

    [Fact]
    internal async Task WhenCreatePostIsCalledWithValidRequest_ThenCorrectValuesAreSaved()
    {
        //Given         
        var existingUser = await _factory.MessageBrokerTestFacade.SendUserCreatedEvent(Guid.NewGuid(), "John Does");
        await _factory.MessageBrokerTestFacade.WaitUntillEventHasBeenConsumed<UserCreatedEvent>(x => x.UserId == existingUser.UserId);

        //When
        var body = PostTestHelper.GetValidDefaultRequest();
        body.CreatorId = existingUser.UserId.ToString(); ;
        var response = await _client.PostAsJsonAsync("/Posts", body);
        var responseContent = await response.Content.ReadFromJsonAsync<CreatePostResponse>();

        //Then
        var getResponse = await _client.GetFromJsonAsync<PostDto>($"/Posts/{responseContent.PostId}");

        Assert.False(string.IsNullOrEmpty(getResponse.Id.ToString()));
        Assert.Equal(body.Title, getResponse.Title);
        Assert.Equal(body.Description, getResponse.Description);
        Assert.Equal(body.CreatorId, getResponse.CreatorId);
        Assert.False(string.IsNullOrEmpty(getResponse.CreatorName));
        Assert.Equal(body.PrimaryColor.ToLower(), getResponse.PrimaryColor.ToString().ToLower());
        Assert.Equal(body.SecondaryColor.ToLower(), getResponse.SecondaryColor.ToString().ToLower());
    }

    //GivenPostExistsWithTwoImages_WhenUserAsksForThePost_ThenResponseContainsTwoImagePaths
    //[Fact]
    //public async Task GivenPostExistsWithTwoImages_WhenUserAsksForPost_thenResponseContainsTwoImagePaths()
    //{
    //    // Given
    //    var existingUser = await SendUserCreatedEvent(Guid.NewGuid(), "John Doe");
    //    await _messageBroker.WaitUntillEventHasBeenConsumed<UserCreatedEvent>(x => x.UserId == existingUser.UserId);

    //    // When
    //    var body = PostTestHelper.GetValidDefaultRequest();
    //    body.CreatorId = existingUser.UserId.ToString();
    //    var response = await _client.PostAsJsonAsync("/Posts", body);
    //    var responseContent = await response.Content.ReadFromJsonAsync<CreateResponse>();

    //    //Then
    //    var collection = _factory.FakeImageBlobStorage.GetDirectory(responseContent.PostId);
    //    Assert.Equal(collection.Count(), body.Images.Count());
    //}

}
