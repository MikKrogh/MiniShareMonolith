using PostsModule.Application;
namespace PostsModule.Domain;

public interface IPostsRepository
{
    Task<Post?> Get(string id);
    Task<PaginationResult<Post>> GetAll(QueryModel query);
    Task Save(Post post);
    Task Delete(string id);
    
}
