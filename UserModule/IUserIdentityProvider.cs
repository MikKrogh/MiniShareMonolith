namespace UserModule
{
    public interface IUserIdentityProvider
    {
        Task<string> CreateUserIdentity(string email, string password);
    }
}
