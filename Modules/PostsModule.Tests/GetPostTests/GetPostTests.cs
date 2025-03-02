using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PostsModule.Application.UserEvents;
using PostsModule.Presentation.Endpoints;
using PostsModule.Tests.CreatePostTests;
using PostsModule.Tests.Helper;
using System.Net.Http.Json;

namespace PostsModule.Tests.GetPostTests;

public class GetPostTests : IClassFixture<PostsWebApplicationFactory>
{
    private readonly MessageBrokerTestFacade _messageBroker;
    private readonly PostsWebApplicationFactory _factory;
    private readonly HttpClient _client;
    public GetPostTests(PostsWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _messageBroker = _factory.Services.GetRequiredService<MessageBrokerTestFacade>();
    }

    //GivenPostExists_WhenUserAsksForThePost_ThenResponseIs200
    [Fact]
    public async Task GivenPostExists_WhenUserAsksForThePost_ThenResponseIs200()
    {
        //Given         
        var existingUser = await _factory.MessageBrokerTestFacade.SendUserCreatedEvent(Guid.NewGuid(), "John Does");
        await _messageBroker.WaitUntillEventHasBeenConsumed<UserCreatedEvent>(x => x.UserId == existingUser.UserId);

        var createBody = PostTestHelper.GetValidDefaultRequest(existingUser.UserId);        
        var response = await _client.PostAsJsonAsync("/Posts", createBody);
        var responseContent = await response.Content.ReadFromJsonAsync<CreatePostResponse>();

        // When
        var getResponse = await _client.GetAsync($"/Posts/{responseContent.PostId}");

        // Then
        Assert.True(getResponse.IsSuccessStatusCode);
    }

    [Fact]
    internal async Task GivenPostExists_WhenUserAsksForPost_ThenCorrectValuesAreReturned()
    {
        // Given         
        var existingUser = await _factory.MessageBrokerTestFacade.SendUserCreatedEvent(Guid.NewGuid(), "John Does");
        await _messageBroker.WaitUntillEventHasBeenConsumed<UserCreatedEvent>(x => x.UserId == existingUser.UserId);

        var createBody = PostTestHelper.GetValidDefaultRequest(existingUser.UserId);
        var response = await _client.PostAsJsonAsync("/Posts", createBody);
        var responseContent = await response.Content.ReadFromJsonAsync<CreatePostResponse>();

        // When
        var getResponse = await _client.GetFromJsonAsync<PostDto>($"/Posts/{responseContent.PostId}");

        // Then
        Assert.False(string.IsNullOrEmpty(getResponse.Id.ToString()));
        Assert.Equal(createBody.Title, getResponse.Title);
        Assert.Equal(createBody.Description, getResponse.Description);
        Assert.Equal(createBody.CreatorId, getResponse.CreatorId);
        Assert.False(string.IsNullOrEmpty(getResponse.CreatorName));
        Assert.Equal(createBody.PrimaryColor.ToLower(), getResponse.PrimaryColor.ToString().ToLower());
        Assert.Equal(createBody.SecondaryColor.ToLower(), getResponse.SecondaryColor.ToString().ToLower());
    }

    [Fact]
    public async Task GivenPostExistsWithTwoImages_WhenUserAsksForPost_thenResponseContainsTwoImagePaths()
    {
        // Given
        var existingUser = await _messageBroker.SendUserCreatedEvent(Guid.NewGuid(), "John Does");
        await _messageBroker.WaitUntillEventHasBeenConsumed<UserCreatedEvent>(x => x.UserId == existingUser.UserId);

        var body = PostTestHelper.GetValidDefaultRequest(existingUser.UserId);
        var formFiles = Enumerable.Range(1, 2).Select(i => PostTestHelper.CreateFormFile($"file{i}.txt"));
        var collection = new FormFileCollection();
        collection.AddRange(formFiles);
        body.Images = collection;

        var response = await _client.PostAsJsonAsync("/Posts", body);
        var responseContent = await response.Content.ReadFromJsonAsync<CreatePostResponse>();

        //When


        //Then
        Assert.True(false);

        //var collection = _factory.FakeImageBlobStorage.GetDirectory(responseContent.PostId);
        //Assert.Equal(collection.Count(), body.Images.Count());
    }

}
