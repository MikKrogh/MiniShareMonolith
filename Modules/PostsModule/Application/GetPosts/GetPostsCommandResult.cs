namespace PostsModule.Application.GetPosts
{
    public class GetPostsCommandResult
    {
        public List<PostDto> Posts { get; private set; } = new List<PostDto>();
    }
}
