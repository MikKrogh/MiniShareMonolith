using Microsoft.Extensions.DependencyInjection;
using PostsModule.Application.UserEvents;
using PostsModule.Presentation.Endpoints;
using PostsModule.Tests.CreatePostTests;
using PostsModule.Tests.Helper;
using System.Net.Http.Json;


namespace PostsModule.Tests.Tests.ImageTests;

public class ImageTests : IClassFixture<PostsWebApplicationFactory>
{
    private readonly MessageBrokerTestFacade _messageBroker;
    private readonly PostsWebApplicationFactory _factory;
    private readonly HttpClient _client;
    public ImageTests(PostsWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _messageBroker = _factory.Services.GetRequiredService<MessageBrokerTestFacade>();
    }

    [Fact]
    public async Task GivenUserHasCreatedNewPost_WhenUserAddsImageToPost_ThenSuccessIsReturned()
    {
        //Given 
        var existingUser = await _messageBroker.SendUserCreatedEvent(Guid.NewGuid(), "John Does");
        await _messageBroker.WaitUntillEventHasBeenConsumed<UserCreatedEvent>(x => x.UserId == existingUser.UserId);

        var body = PostRequestBuilder.GetValidDefaultRequest(existingUser.UserId);
        var createPostResponse = await _client.PostAsJsonAsync("/Posts", body);
        var createResponseContent = await createPostResponse.Content.ReadFromJsonAsync<CreatePostResponse>();

        //When
        var response = await _client.PostAsync($"/Posts/{createResponseContent.PostId}/Image?token={createResponseContent.Token}", FakeImageContent());

        //Then
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task GivenUserHasCreatedNewPost_WhenUserAddsImageToPost_ThenPostContainsImagePath()
    {
        //Given 
        var existingUser = await _messageBroker.SendUserCreatedEvent();
        await _messageBroker.WaitUntillEventHasBeenConsumed<UserCreatedEvent>(x => x.UserId == existingUser.UserId);

        var body = PostRequestBuilder.GetValidDefaultRequest(existingUser.UserId);
        var createPostResponse = await _client.PostAsJsonAsync("/Posts", body);
        var createResponseContent = await createPostResponse.Content.ReadFromJsonAsync<CreatePostResponse>();
        
        //When
        var response = await _client.PostAsync($"/Posts/{createResponseContent.PostId}/Image?token={createResponseContent.Token}", FakeImageContent());
        
        //Then
        var getResponse = await _client.GetFromJsonAsync<PostDto>($"/Posts/{createResponseContent.PostId}");

        Assert.NotEmpty(getResponse.Images);
        Assert.Equal(getResponse.Images.Count(), 1);
    }



    public HttpContent FakeImageContent()
    {
        var bytes = new byte[123456789];
        var stream = new MemoryStream(bytes);
        var form = new MultipartFormDataContent();
        form.Add(new StreamContent(stream), "file", $"filename.jpeg");
        return form;
    }
}
