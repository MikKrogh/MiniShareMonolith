using Microsoft.Extensions.DependencyInjection;
using PostsModule.Application;
using PostsModule.Presentation;
using PostsModule.Tests.Helper;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace PostsModule.Tests;

internal class TestFacade
{
    private readonly PostsWebApplicationFactory _factory;
    public readonly MesageBrokerFacade MessageBroker;
    

    public readonly HttpClient _client;
    public TestFacade(PostsWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        MessageBroker = factory.Services.GetRequiredService<MesageBrokerFacade>();
        

    }

    public void TruncateTables()
    {
        _factory.TruncateTables();
    }

    public async Task<UserCreatedEvent> SendCreateUserEvent()
    {
        var id = Guid.NewGuid();
        string username = id.ToString().Substring(0, 8);
        var existingUser = await MessageBroker.SendUserCreatedEvent(id, username);
        return existingUser;
    }

    public async Task<TestFacadeResult<CreatePostResponse?>> SendCreatePost(PostRequest request, string? userId)
    {
        int maxRetries = 7;
        int attempts = 0;
        HttpStatusCode latestStatusCode = default;
        while (attempts < maxRetries)
        {
            attempts++;
            try
            {
                var response = await _client.PostAsJsonAsync($"/Posts?UserId={userId}", request);
                latestStatusCode = response.StatusCode;
                if (response.IsSuccessStatusCode)
                {
                    return new TestFacadeResult<CreatePostResponse?>
                    {
                        StatusCode = response.StatusCode,
                        Result = response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<CreatePostResponse>() : null
                    };
                }
            }
            catch (Exception e)
            {
                if (attempts >= maxRetries) throw;
                await Task.Delay(100 * attempts);
            }
        }
        return new TestFacadeResult<CreatePostResponse?>
        {
            StatusCode = latestStatusCode,
            Result = null
        };
    }

    public async Task<HttpStatusCode> DeletePost(string postId, string userId)
    {
        var response = await _client.DeleteAsync($"/Posts/{postId}?userId={userId}");
        return response.StatusCode;
    }

    public async Task<PostDto?> GetPost(string id)
    {
        var t =  await _client.GetFromJsonAsync<PostDto?>($"/Posts/{id}");
        return t;
    }


    public async Task<TestFacadeResult<PaginationResult<PostDto>>> GetPosts(string? queryString = null)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };


        var result = new TestFacadeResult<PaginationResult<PostDto>>()
        {
            Result = new PaginationResult<PostDto>()
        };

        var response = await _client.GetAsync($"/Posts?{queryString}");
        result.StatusCode = response.StatusCode;
        if (!response.IsSuccessStatusCode) return result;

        var content = await response.Content.ReadAsStringAsync();
        var listofDto = string.IsNullOrEmpty(content)
            ? new PaginationResult<PostDto>()
            : JsonSerializer.Deserialize<PaginationResult<PostDto>>(content, options);

        result.Result = listofDto;
        return result;
    }




}
public class TestFacadeResult<T>
{
    public T? Result { get; set; }
    public HttpStatusCode StatusCode { get; set; }

}
public class CreatePostResponse
{
    public string PostId { get; set; }
    public string Token { get; set; }
}
public record UserCreatedEvent
{
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
}