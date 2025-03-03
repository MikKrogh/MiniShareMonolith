using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PostsModule.Application.UserEvents;
using PostsModule.Presentation.Endpoints;
using PostsModule.Tests.CreatePostTests;
using PostsModule.Tests.Helper;
using System.Net.Http.Json;
using System.Text;

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

        var formdata = new MultipartFormDataContent();

        var body = PostTestHelper.GetValidDefaultRequest(existingUser.UserId);
        var jsonbody = System.Text.Json.JsonSerializer.Serialize(body);
        formdata.Add(new StringContent(jsonbody, Encoding.UTF8, "application/json"), "jsonData"); // "jsonData" is the key name

        var formFiles = Enumerable.Range(1, 2).Select(i => PostTestHelper.CreateFormFile($"file{i}.txt"));
        foreach (var formFile in formFiles)
        {
            formdata.Add(ConvertToHttpContent(formFile), "images", formFile.FileName); // "images" is the key name
        }
        var collection = new FormFileCollection();

        
        collection.AddRange(formFiles);
        body.Images = collection;
       
        var response = await _client.PostAsync("/Posts", formdata);
        var responseContent = await response.Content.ReadFromJsonAsync<CreatePostResponse>();

        //When
        var getResponse = await _client.GetFromJsonAsync<PostDto>($"/Posts/{responseContent.PostId}");

        //Then
        Assert.Equal(formFiles.Count(), getResponse.Images.Count());
    }

    //VaildationOnFormFiles i create, filesize,  file type etc etc. check with gtp what other checks should be done
    public static HttpContent ConvertToHttpContent(IFormFile formFile)
    {
        // Read the file's content into a byte array
        using (var memoryStream = new MemoryStream())
        {
            // Copy the file content to the memory stream
            formFile.CopyTo(memoryStream);

            // Create a ByteArrayContent instance with the byte array
            var byteArrayContent = new ByteArrayContent(memoryStream.ToArray());

            // Set the content type (optional, but should be done based on your file type)
            byteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(formFile.ContentType);

            // Return the ByteArrayContent
            return byteArrayContent;
        }
    }
}
