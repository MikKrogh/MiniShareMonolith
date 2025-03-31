using System.Net.Http.Json;

namespace UserModule.Tests.Tests;

public class GetUserTests : IClassFixture<UserWebApplicationFactory>
{
    HttpClient client;
    public GetUserTests(UserWebApplicationFactory factory)
    {
        client = factory.CreateClient();
    }


    [Fact]
    public async Task GivenUserExists_WhenSomeoneTriesToQueryUser_ThenSuccessIsReturned()
    {
        // Given
        var requestBody = UserBuilder.CreateValidUserBody();
        var response = await client.PostAsJsonAsync("User", requestBody);

        // When
        var getResponse = await client.GetAsync($"User/{requestBody.UserId}");

        // Then
        Assert.NotNull( getResponse );
        Assert.True(getResponse.IsSuccessStatusCode);

    }
    [Fact]
    public async Task GivenUserExists_WhenSomeoneTriesToQueryUser_ThenCorrectValuesAreReturned()
    {
        // Given
        var requestBody = UserBuilder.CreateValidUserBody();
        var response = await client.PostAsJsonAsync("User", requestBody);

        // When
        var getResponse = await client.GetFromJsonAsync<User>($"User/{requestBody.UserId}");

        // Then
        Assert.NotNull(getResponse);
        Assert.Equal(requestBody.UserId, getResponse.Id);
        Assert.Equal(requestBody.UserName, getResponse.UserName);
        DateTime now = DateTime.UtcNow;
        Assert.True((now - getResponse.CreationDate).TotalMilliseconds <= 500, $"Expected {getResponse.CreationDate} to be within 500ms of {now}");
    }


    [Fact]
    public async Task GivenNoUserExists_WhenGetUserIsCalled_ThenNotFoundIsReturned()
    {
        var getResponse = await client.GetAsync($"User/{Guid.NewGuid()}");

        // Then
        Assert.NotNull(getResponse);
        Assert.Equal(System.Net.HttpStatusCode.NotFound,getResponse.StatusCode);
    }
}