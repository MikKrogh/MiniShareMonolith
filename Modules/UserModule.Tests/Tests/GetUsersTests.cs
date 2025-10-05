using System.Net.Http.Json;

namespace UserModule.Tests.Tests;

public class GetUsersTests : IClassFixture<UserWebApplicationFactory>
{
    HttpClient client;
    public GetUsersTests(UserWebApplicationFactory factory)
    {
        client = factory.CreateClient();
    }


    [Fact]
    public async Task GivenUsersExist_WheBatchOfUsersIsRequested_ThenSuccessIsReturned()
    {
        var createdUsers = await CreateUsers(10);
        var concatenatedIds = string.Join(",", createdUsers.Select(x => x.UserId));       
        
        // When
        var getResponse = await client.GetAsync($"Users/{concatenatedIds}");

        // Then
        Assert.NotNull(getResponse);
        Assert.True(getResponse.IsSuccessStatusCode, $"Expected success status code but got {getResponse.StatusCode}");
    }

    [Fact]
    public async Task GivenUsersExists_WhenBatchOfUsersIsRequested_ThenCorrectValuesAreReturned()
    {
        var createdUsers = await CreateUsers(10);
        var concatenatedIds = string.Join(",", createdUsers.Select(x => x.UserId));
        // When
        var getResponse = await client.GetFromJsonAsync<List<User>>($"Users/{concatenatedIds}");
        // Then
        Assert.NotNull(getResponse);
        Assert.Equal(createdUsers.Count, getResponse.Count);
        foreach (var user in getResponse)
        {
            var createdUser = createdUsers.FirstOrDefault(x => x.UserId == user.Id);
            Assert.NotNull(createdUser);
            Assert.Equal(createdUser.UserName, user.UserName);
            DateTime now = DateTime.UtcNow;
            Assert.True((now - user.CreationDate).TotalMilliseconds <= 750, $"Expected {user.CreationDate} to be within 500ms of {now}");
        }
    }
    [Fact]
    public async Task GivenNoUsersMatchQuery_WhenBatchOfUsersIsRequested_ThenEmptySuccessIsReturned()
    {
        // Given        
        var concatenatedIds = string.Join(",", Enumerable.Range(0,10).Select(x => Guid.NewGuid().ToString()));
        // When
        var getResponse = await client.GetFromJsonAsync<List<User>>($"Users/{concatenatedIds}");
        // Then
        Assert.NotNull(getResponse);
        Assert.Empty(getResponse);
    }


    private async Task<List<CreatedUser>> CreateUsers(int count)
    {
        var createdUsers = new List<CreatedUser>();
        for (int i = 0; i < count; i++)
        {
            var user = UserBuilder.GenerateUserToCreate();
            await client.SendCreateUserRequest(user);
            createdUsers.Add(new CreatedUser(user.UserId, user.DisplayName));
        }
        return createdUsers;
    }
    private class CreatedUser
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public CreatedUser(string userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }
    }

}
