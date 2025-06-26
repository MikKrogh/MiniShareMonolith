namespace PostsModule.Domain;

public interface IPostsRepository
{
    Task<Post?> Get(string id);
    Task<string?> GetCreatorId(string postId);
    Task<PaginatedResult<Post>> GetAll(int take = 100, bool? descending = null, string? orderOnProperty = null, string? filter = null, string? search = null, int skip = 0);
    Task Save(Post post);
    Task Delete(string id);
}
