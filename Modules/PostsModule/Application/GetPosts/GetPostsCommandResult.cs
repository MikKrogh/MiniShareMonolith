namespace PostsModule.Application.GetPosts;

public sealed class GetPostsCommandResult
{
    public int TotalCount { get; init; } = 0;

    public List<PostDto> Posts { get; init; } = new List<PostDto>();
}
