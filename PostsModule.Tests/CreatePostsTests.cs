
using Microsoft.AspNetCore.Mvc.Testing;
using PostsModule.Presentation.Endpoints;
using PostsModule.Tests.Helper;
using System.Net.Http.Json;

namespace PostsModule.Tests;

public class CreatePostsTests : IClassFixture<WebApplicationFactory<Program>>
{
	WebApplicationFactory<Program> _factory;
	HttpClient _client;
	public CreatePostsTests(WebApplicationFactory<Program> factory)
	{
		_factory = factory;
		_client = factory.CreateClient();
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
	internal async Task WhenCreatePostIsCalledWithValidRequest_ThenResponseContainsId()
	{
		//When
		var body = PostTestHelper.GetValidDefaultRequest();
		var response = await _client.PostAsJsonAsync("/Posts", body);

		//Then
		var id = await response.Content.ReadAsStringAsync();
		Assert.False(string.IsNullOrEmpty(id));
	}
	[Fact]
	internal async Task WhenCreatePostIsCalledWithValidRequest_ThenCorrectValuesAreSaved()
	{
		//When
		var body = new PostsRequestBuilder().Create()
			.WithTitle("someTitle")
			.WithDescription("someDesc")
			.WithMainColor("Red")
			.WithSecondaryColor("blue")
			.Build();
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


}
public class CreateResponse
{

	public string PostId { get; set; }
}
