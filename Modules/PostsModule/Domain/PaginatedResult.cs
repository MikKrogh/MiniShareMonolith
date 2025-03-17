namespace PostsModule.Domain;

public class PaginatedResult<T> where T : class
{
    public List<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; } = 0;
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

}
