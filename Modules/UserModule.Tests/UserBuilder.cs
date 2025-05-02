

using System.Net.Http.Json;

namespace UserModule.Tests;


internal static class UserBuilder
{
    internal class UserToCreate
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
    }

    public static UserToCreate GenerateUserToCreate(string? userId = null, string? userName = null)
    {
        userId = string.IsNullOrEmpty(userId) ? Guid.NewGuid().ToString() : userId;
        userName = string.IsNullOrEmpty(userName) ? Guid.NewGuid().ToString() : userName;   
        userName = $"UserName{userName.Substring(0, 8)}";
        var result = new UserToCreate
        {
            UserId = userId,
            DisplayName = userName
        };
        return result;
    }

    public static  async Task<HttpResponseMessage> SendCreateUserRequest(this HttpClient client, UserToCreate user)
    {
        var response = await client.PostAsJsonAsync($"User?userId={user.UserId}", new { DisplayName = user.DisplayName });
        return response;
    }
}


