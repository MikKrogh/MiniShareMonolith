namespace PostsModule.Domain;

public interface IPostsRepository
{
	Task<Post?> Get(string id);
    Task Save(Post post);
	Task Delete(string id);
}

public interface IUserRepository
{
	Task Create(User user);

	
	Task Update(User user);
	Task Delete(string userId);
}
