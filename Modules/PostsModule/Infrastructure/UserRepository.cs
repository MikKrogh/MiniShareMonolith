using PostsModule.Domain;

namespace PostsModule.Infrastructure;

internal class UserRepository : IUserRepository
{
    private readonly PostsContext context;

    public UserRepository( PostsContext context)
    {
        this.context = context;
    }

    public async Task Create(User user)
    {
        var userEntity = new UserEntity
        {
            Id = user.Id.ToString(),
            UserName = user.Name,
        };
        await context.Users.AddAsync(userEntity);
        await context.SaveChangesAsync();
    }

    public Task Delete(string userId)
    {
        throw new NotImplementedException();
    }

    public Task Update(User user)
    {
        throw new NotImplementedException();
    }
}
