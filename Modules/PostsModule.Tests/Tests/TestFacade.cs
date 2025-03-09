

using Microsoft.Extensions.DependencyInjection;
using PostsModule.Application.UserEvents;
using PostsModule.Presentation.Endpoints;
using PostsModule.Tests.CreatePostTests;
using PostsModule.Tests.Helper;
using System.Net;
using System.Net.Http.Json;

namespace PostsModule.Tests.Tests;

internal class TestFacade
{
    private readonly MessageBrokerTestFacade _messageBroker;

    private readonly HttpClient _client;
    public TestFacade(PostsWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _messageBroker = factory.Services.GetRequiredService<MessageBrokerTestFacade>();
    }

    public async Task<UserCreatedEvent> SendCreateUserEvent(string? name = null)
    {
        var existingUser = await _messageBroker.SendUserCreatedEvent(Guid.NewGuid(), name ?? "John Doe");
        await _messageBroker.WaitUntillEventHasBeenConsumed<UserCreatedEvent>(x => x.UserId == existingUser.UserId);
        return existingUser;
    }

    public async Task<TestFacadeResult<CreatePostResponse?>> SendCreatePost(PostRequest request)
    {
        var response = await _client.PostAsJsonAsync("/Posts", request);

        return new TestFacadeResult<CreatePostResponse?>
        {
            StatusCode = response.StatusCode,
            Result = response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<CreatePostResponse>() : null
        };
    }

    public async Task<HttpStatusCode> UploadImage(string postId, string token)
    {
        var bytes = new byte[123456789];
        var stream = new MemoryStream(bytes);
        var form = new MultipartFormDataContent();
        form.Add(new StreamContent(stream), "file", $"filename.jpeg");

        var response = await _client.PostAsync($"/Posts/{postId}/Image?token={token}", form);
        return response.StatusCode;

    }

    public async Task<PostDto?> GetPost(Guid id) => await _client.GetFromJsonAsync<PostDto>($"/Posts/{id.ToString()}");
    public async Task<PostDto?> GetPost(string id) => await _client.GetFromJsonAsync<PostDto>($"/Posts/{id}");

    public class TestFacadeResult<T>
    {
        public T? Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }

    }
}
public class CreatePostResponse
{
    public string PostId { get; set; }
    public string Token { get; set; }
}
