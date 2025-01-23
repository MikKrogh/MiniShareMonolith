namespace PostsModule.Domain;

public interface IPostsRepository
{
	Task<Post?> Get(string id);
    Task Save(Post post);
	Task Delete(string id);
}
