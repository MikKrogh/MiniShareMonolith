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
        //When
        var body = PostTestHelper.GetValidDefaultRequest();
        var response = await _client.PostAsJsonAsync("/Posts", body);

        //Then
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    internal async Task WhenValidCreateRequestIsMade_ThenCreatedPostIdIsReturned()
    {
        //Given 
        var existingUser = await SendUserCreatedEvent(Guid.NewGuid(), "John Does");
        await _messageBroker.WaitUntillEventHasBeenConsumed <UserCreatedEvent>();


        //When
        var body = PostTestHelper.GetValidDefaultRequest(existingUser.UserId);        
        var response = await _client.PostAsJsonAsync("/Posts", body);

        //Then
        var id = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrEmpty(id));
    }
    [Fact]
    internal async Task WhenCreatePostIsCalledWithValidRequest_ThenCorrectValuesAreSaved()
    {
        //Given         
        var existingUser = await SendUserCreatedEvent(Guid.NewGuid(), "John Does");        

        //When
        var body = PostTestHelper.GetValidDefaultRequest();
        body.CreatorId = existingUser.UserId.ToString(); ;
        var response = await _client.PostAsJsonAsync("/Posts", body);
        var responseContent = await response.Content.ReadFromJsonAsync<CreateResponse>();

        //Then
        var getResponse = await _client.GetFromJsonAsync<PostDto>($"/Posts/{responseContent.PostId}");

        Assert.False(string.IsNullOrEmpty(getResponse.Id.ToString()));
        Assert.Equal(body.Title, getResponse.Title);
        Assert.Equal(body.Description, getResponse.Description);
        Assert.Equal(body.CreatorId, getResponse.CreatorId);
        Assert.False(string.IsNullOrEmpty(getResponse.CreatorName));
        Assert.Equal(body.PrimaryColor, getResponse.PrimaryColor.ToString());
        Assert.Equal(body.SecondaryColor, getResponse.SecondaryColor.ToString());

    }

    [Fact]
    public async Task WhenCreatePostIsCalledWithValidRequest_ThenImagesAreSaved()
    {
        // When
        var body = PostTestHelper.GetValidDefaultRequest();         
        var response = await _client.PostAsJsonAsync("/Posts", body);
        var responseContent = await response.Content.ReadFromJsonAsync<CreateResponse>();

        //Then
        var collection = _factory.FakeImageBlobStorage.GetDirectory(responseContent.PostId);
        Assert.Equal(collection.Count(),body.Images.Count());
    }

    private async Task<UserCreatedEvent> SendUserCreatedEvent(Guid userId, string? username = null)
    {
        var userCreateEvent = new UserCreatedEvent(userId, username ?? "some random Name");
        await _messageBroker.Publish(userCreateEvent);

        //var predicate = new Predicate<UserCreatedEvent>(x => 
        //    x.UserId == userCreateEvent.UserId &&
        //    x.UserName == userCreateEvent.UserName
        //);

        //bool createUserEventIsHandeled = false;
        //while (!createUserEventIsHandeled) 
        //{
        //    createUserEventIsHandeled = await _messageBroker.AssertExactlyOneMessageMatch(predicate);
        //    await Task.Delay(50);
        //}
        return userCreateEvent;
    }
}
public class CreateResponse
{
    public string PostId { get; set; }
}
