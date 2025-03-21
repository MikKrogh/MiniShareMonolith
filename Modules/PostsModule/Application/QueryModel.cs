
namespace PostsModule.Application;
public sealed record QueryModel
{
    public string? Search { get; init; }
    public int Take { get; init; } = 100;
    public int Skip { get; init; } = 0;
    public string? Filter { get; init; } = null;
    public string? OrderBy { get; init; } = string.Empty;
    public bool Descending { get; init; } = false;
}


