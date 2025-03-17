
using Microsoft.Extensions.DependencyInjection;
using PostsModule.Application;
using PostsModule.Application.UserEvents;
using PostsModule.Domain.Auth;
using PostsModule.Presentation;
using PostsModule.Tests.Helper;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace PostsModule.Tests;

internal class TestFacade
{
    private readonly PostsWebApplicationFactory _factory;
    private readonly MesageBrokerFacade _messageBroker;
    private readonly IAuthHelper jwtHandler;    

    private readonly HttpClient _client;
    public TestFacade(PostsWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _messageBroker = factory.Services.GetRequiredService<MesageBrokerFacade>();
        jwtHandler = factory.Services.GetRequiredService<IAuthHelper>();

    }

    public string CreateToken(DateTime? expirationDate, string postId)
    {
        var token = jwtHandler.CreateToken(expirationDate, ClaimValueHolder.Create("postId", postId));
        
        return token;
    }
    public void TruncateTables( )
    {
        _factory.TruncateTables();
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

    public async Task<HttpStatusCode> UploadImage(string postId, string token, byte[]? file = null, string fileExtension = ".jpg")
    {
        var bytes = file ?? new byte[2000];
        var stream = new MemoryStream(bytes);
        var form = new MultipartFormDataContent();
        form.Add(new StreamContent(stream), "file", $"filename" + fileExtension);

        var response = await _client.PutAsync($"/Posts/{postId}/Image?token={token}", form);
        return response.StatusCode;
    }

    public async Task<PostDto?> GetPost(Guid id) => await _client.GetFromJsonAsync<PostDto>($"/Posts/{id.ToString()}");
    public async Task<PostDto?> GetPost(string id) => await _client.GetFromJsonAsync<PostDto>($"/Posts/{id}");
    public async Task<TestFacadeResult<byte[]>> GetImage(string postId, string imageId)
    {
        var result = new TestFacadeResult<byte[]>();

        var response = await _client.GetAsync($"/Posts/{postId}/image/{imageId}");
        result.StatusCode = response.StatusCode;

        if (!response.IsSuccessStatusCode)
        {
            result.Result = null;
            return result;
        }

        var content = response.Content.ReadAsByteArrayAsync();
        result.Result = await content;
        return result;
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
