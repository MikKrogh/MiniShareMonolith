
namespace PostsModule.Application
{
    public class QueryModel
    {
        public string? Search {  get; set; }
        public int Take { get; set; } = 100;
        public int Skip { get; set; } = 0;
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = string.Empty;
        public bool Descending { get; set; } = false;
    }
}


