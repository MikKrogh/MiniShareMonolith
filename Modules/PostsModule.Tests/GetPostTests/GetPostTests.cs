using PostsModule.Presentation.Endpoints;
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
        var createBody = PostTestHelper.GetValidDefaultRequest();
        var createResponse = await _client.PostAsJsonAsync("/Posts", createBody);
        var createdPostId = await createResponse.Content.ReadAsStringAsync();
        // When

        var getResponse = await _client.GetFromJsonAsync<PostDto>($"/Posts/b58335f9-6f4a-4123-8354-2da3318debac");

        // Then
    }


    //GivenPostExists_WhenUserAsksForThePost_ThenResponseContainsCorrectValues
    //GivenPostExistsWithTwoImages_WhenUserAsksForThePost_ThenResponseContainsTwoImagePaths

}
