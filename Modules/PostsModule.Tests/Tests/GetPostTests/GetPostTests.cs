using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Server;
using PostsModule.Application.UserEvents;
using PostsModule.Presentation;
using PostsModule.Presentation.Endpoints;
using PostsModule.Tests.CreatePostTests;
using PostsModule.Tests.Helper;
using PostsModule.Tests.Tests;
using System;
using System.Collections;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace PostsModule.Tests.GetPostTests;

public class GetPostTests : IClassFixture<PostsWebApplicationFactory>
{
    private readonly MessageBrokerTestFacade _messageBroker;
    private readonly PostsWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly TestFacade testFacade;
    public GetPostTests(PostsWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _messageBroker = _factory.Services.GetRequiredService<MessageBrokerTestFacade>();
        testFacade = new TestFacade(factory);
    }

    [Fact]
    public async Task GivenPostExists_WhenUserAsksForThePost_ThenResponseIs200()
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();


        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);        
        var create = await testFacade.SendCreatePost(createBody);

        // When
        var post = await _client.GetAsync($"/Posts/{create.Result.PostId}");

        // Then
        Assert.True(post.IsSuccessStatusCode);
    }

    [Fact]
    internal async Task GivenPostExists_WhenUserAsksForPost_ThenCorrectValuesAreReturned()
    {
        // Given         
        var existingUser = await _factory.MessageBrokerTestFacade.SendUserCreatedEvent(Guid.NewGuid(), "John Does");
        await _messageBroker.WaitUntillEventHasBeenConsumed<UserCreatedEvent>(x => x.UserId == existingUser.UserId);

        var createBody = PostRequestBuilder.GetValidDefaultRequest(existingUser.UserId);
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
        Assert.True(false);
    }
}
