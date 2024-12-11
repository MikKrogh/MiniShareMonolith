using PostsModule.Domain;

namespace PostsModule.Infrastructure;

public class PostsRepository : IPostsRepository
{
    public Task Delete(string id)
    {
        throw new NotImplementedException();
    }

    public Task Save(Post post)
    {
        throw new NotImplementedException();
    }
}
