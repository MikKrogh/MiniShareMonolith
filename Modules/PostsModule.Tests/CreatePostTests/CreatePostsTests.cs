using Microsoft.Extensions.DependencyInjection;
using PostsModule.Application.UserEvents;
using PostsModule.Presentation.Endpoints;
using PostsModule.Tests.Helper;
using System.Net.Http.Json;

namespace PostsModule.Tests.CreatePostTests;

public class CreatePostsTests : IClassFixture<PostsWebApplicationFactory>
{
    private readonly MessageBrokerTestFacade _messageBroker;
    private readonly PostsWebApplicationFactory _factory;
    private readonly HttpClient _client;
    public CreatePostsTests(PostsWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _messageBroker = _factory.Services.GetRequiredService<MessageBrokerTestFacade>();
    }


    [Fact]
    internal async Task WhenCreatePostIsCalledWithValidRequest_ThenSuccessIsReturned()
    {
        // Given
        var existingUser = await _messageBroker.SendUserCreatedEvent(Guid.NewGuid(), "John Doe");
        await _messageBroker.WaitUntillEventHasBeenConsumed<UserCreatedEvent>(x => x.UserId == existingUser.UserId);

        //When
        var body = PostTestHelper.GetValidDefaultRequest();
        body.CreatorId = existingUser.UserId.ToString();
        var response = await _client.PostAsJsonAsync("/Posts", body);

        //Then
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    internal async Task WhenValidCreateRequestIsMade_ThenCreatedPostIdIsReturned()
    {
        //Given 
        var existingUser = await _messageBroker.SendUserCreatedEvent(Guid.NewGuid(), "John Does");
        await _messageBroker.WaitUntillEventHasBeenConsumed<UserCreatedEvent>(x => x.UserId == existingUser.UserId);


        //When
        var body = PostTestHelper.GetValidDefaultRequest(existingUser.UserId);        
        var response = await _client.PostAsJsonAsync("/Posts", body);

        //Then
        var id = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrEmpty(id));
    }

}
