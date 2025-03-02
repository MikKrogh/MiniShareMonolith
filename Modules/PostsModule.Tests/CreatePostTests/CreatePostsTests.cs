using Microsoft.Extensions.DependencyInjection;
using PostsModule.Application.UserEvents;
using PostsModule.Presentation.Endpoints;
using PostsModule.Tests.Helper;
using System.Net;
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
    internal async Task GivenUserExists_WhenUserCreatesPost_ThenSuccessIsReturned()
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
    internal async Task GivenUserExists_WhenUserCreatesPost_ThenNewPostIdIsReturned()
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

    [Fact]
    public async Task GivenNoUserExist_WhenSomeoneCreatesPost_ThenInternalServerErrorIsReturned()
    {
        // When
        var body = PostTestHelper.GetValidDefaultRequest(Guid.NewGuid());
        var response = await _client.PostAsJsonAsync("/Posts", body);

        //Then
        Assert.Equal(response.StatusCode, System.Net.HttpStatusCode.InternalServerError);
    }


    [Theory,]
    [InlineData("")]
    [InlineData("Abclkjsd")]
    [InlineData("12365487654")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("ghfd777t-dfgh-987g-hgc6-gvjhg8976ygh")]
    [InlineData(null)]
    
    public async Task WhenSomeoneCreatesPostWithNonValidCreatorId_ThenBadRequestIsReturned(string? creatorId)
    {
        //When
        var body = PostTestHelper.GetValidDefaultRequest();
        body.CreatorId = creatorId;
        var response = await _client.PostAsJsonAsync("/Posts", body);

        //Then
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("min")]
    [InlineData("beyondMaxCharCountofThirtytwoXxXx")]
    [InlineData("stringWithNumeric5")]
    [InlineData("12365487654")]
    public async Task WhenSomeoneCreatesPostWithNonValidTitle_ThenBadRequestIsReturned(string? title)
    {
        var body = PostTestHelper.GetValidDefaultRequest();
        body.Title = title;        
        var response = await _client.PostAsJsonAsync("/Posts", body);

        //Then
        Assert.True(HttpStatusCode.BadRequest == response.StatusCode, $"assertation against title with value: {title}, response was satus code: {response.StatusCode}");
        
    }


    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("min")]
    [InlineData("stringWithNumeric5")]
    [InlineData("maxcountof25reachedXxXxXxX")]

    public async Task WhenSomeoneCreatesPostWithNonValidFaction_ThenBadRequestIsReturned(string? factionName)
    {
        var body = PostTestHelper.GetValidDefaultRequest();
        body.FactionName = factionName;
        var response = await _client.PostAsJsonAsync("/Posts", body);

        //Then
        Assert.True(HttpStatusCode.BadRequest == response.StatusCode, $"assertation against title with value: {factionName}, response was of type{response.StatusCode}");
    }

    [Theory]
    [InlineData("rEd")]
    [InlineData("reD")]
    [InlineData("RED")]
    [InlineData("red")]
    [InlineData("BLue")]
    [InlineData("BluE")]
    public async Task GivenUserExists_WhenUserCreatedPostWithWierdCasingForPrimaryColor_ThenSuccessIsReturnedAndColorIsSavedWithNormalName(string primaryColor)
    {
        //Given 
        var existingUser = await _messageBroker.SendUserCreatedEvent(Guid.NewGuid(), "John Does");
        await _messageBroker.WaitUntillEventHasBeenConsumed<UserCreatedEvent>(x => x.UserId == existingUser.UserId);

        //When
        var body = PostTestHelper.GetValidDefaultRequest(existingUser.UserId);
        body.PrimaryColor = primaryColor;
        var response = await _client.PostAsJsonAsync("/Posts", body);
        var responseContent = await response.Content.ReadFromJsonAsync<CreatePostResponse>();

        //Then
        var getResponse = await _client.GetFromJsonAsync<PostDto>($"/Posts/{responseContent.PostId}");
        Assert.True(response.StatusCode == HttpStatusCode.OK);

        Assert.True(char.IsUpper(getResponse.PrimaryColor.ToString()[0]));
        Assert.True(getResponse.PrimaryColor.ToString().Skip(1).All(char.IsLower));
    }

    [Theory]
    [InlineData("rEd")]
    [InlineData("reD")]
    [InlineData("RED")]
    [InlineData("red")]
    [InlineData("BLue")]
    [InlineData("BluE")]
    public async Task GivenUserExists_WhenUserCreatedPostWithWierdCasingForSecondaryColor_ThenSuccessIsReturnedAndColorIsSavedWithNormalName(string secondaryColor)
    {
        //Given 
        var existingUser = await _messageBroker.SendUserCreatedEvent(Guid.NewGuid(), "John Does");
        await _messageBroker.WaitUntillEventHasBeenConsumed<UserCreatedEvent>(x => x.UserId == existingUser.UserId);

        //When
        var body = PostTestHelper.GetValidDefaultRequest(existingUser.UserId);
        body.SecondaryColor = secondaryColor;
        var response = await _client.PostAsJsonAsync("/Posts", body);
        var responseContent = await response.Content.ReadFromJsonAsync<CreatePostResponse>();

        //Then
        var getResponse = await _client.GetFromJsonAsync<PostDto>($"/Posts/{responseContent.PostId}");
        Assert.True(response.StatusCode == HttpStatusCode.OK);

        Assert.True(char.IsUpper(getResponse.SecondaryColor.ToString()[0]));
        Assert.True(getResponse.SecondaryColor.ToString().Skip(1).All(char.IsLower));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("bogus")]
    [InlineData("r3d")]

    public async Task WhenSomeoneCreatesPostWithNonValidPrimaryColor_ThenSuccessIsReturnedAndPostIsSavedWithUndefinedPrimaryColor(string? primaryColor)
    {
        // Given
        var existingUser = await _messageBroker.SendUserCreatedEvent(Guid.NewGuid(), "John Doe");
        await _messageBroker.WaitUntillEventHasBeenConsumed<UserCreatedEvent>(x => x.UserId == existingUser.UserId);

        //When
        var body = PostTestHelper.GetValidDefaultRequest(existingUser.UserId);
        body.PrimaryColor = primaryColor;            
        var response = await _client.PostAsJsonAsync("/Posts", body);
        var responseContent = await response.Content.ReadFromJsonAsync<CreatePostResponse>();

        //Then
        var getResponse = await _client.GetFromJsonAsync<PostDto>($"/Posts/{responseContent.PostId}");
        Assert.True(Domain.Colors.Unknown == getResponse.PrimaryColor, $"assertation against primary color with value: {primaryColor}");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("bogus")]
    [InlineData("r3d")]
    public async Task WhenSomeoneCreatesPostWithNonValidSecondaryColor_ThenSuccessIsReturnedAndPostIsSavedWithUndefinedPrimaryColor(string? secondaryColor)
    {
        // Given
        var existingUser = await _messageBroker.SendUserCreatedEvent(Guid.NewGuid(), "John Doe");
        await _messageBroker.WaitUntillEventHasBeenConsumed<UserCreatedEvent>(x => x.UserId == existingUser.UserId);

        //When
        var body = PostTestHelper.GetValidDefaultRequest(existingUser.UserId);
        body.SecondaryColor = secondaryColor;
        var response = await _client.PostAsJsonAsync("/Posts", body);
        var responseContent = await response.Content.ReadFromJsonAsync<CreatePostResponse>();

        //Then
        var getResponse = await _client.GetFromJsonAsync<PostDto>($"/Posts/{responseContent.PostId}");
        Assert.True(Domain.Colors.Unknown == getResponse.SecondaryColor, $"assertation against primary color with value: {secondaryColor}");
    }



}
