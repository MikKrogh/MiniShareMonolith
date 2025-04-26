namespace PostsModule.Presentation.Endpoints;
public class CreateBody
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FactionName { get; set; } = string.Empty;
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
}
