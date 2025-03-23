namespace UserModule.Tests;

class FakeIdentityProvider : IUserIdentityProvider
{
    private List<CreatedUser> _createdUsers = new();
    public Task<string> CreateUserIdentity(string email, string password)
    {
        var user = new CreatedUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            Password = password
        };
        _createdUsers.Add(user);
        return Task.FromResult(user.Id);
    }

    private class CreatedUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
