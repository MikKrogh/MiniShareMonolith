namespace PostsModule.Presentation;

public class PaginationResult<T> where T : class
{
    public IEnumerable<T> Items { get; set; }  = Enumerable.Empty<T>();
    public int TotalCount { get; init; } = 0;
}
