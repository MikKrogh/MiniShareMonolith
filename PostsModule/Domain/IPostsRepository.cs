namespace PostsModule.Domain;

public interface IPostsRepository
{
	Task Save(Post post);
	Task Delete(string id);
}
