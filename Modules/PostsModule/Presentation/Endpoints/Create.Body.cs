namespace PostsModule.Presentation.Endpoints;
internal class CreateBody
{
	public string Title {  get; set; } = string.Empty;
    public string CreatorId { get; set; } = string.Empty;
    public string? Description { get; set; }
	public string FactionName { get; set; } = string.Empty;
    public string? PrimaryColour { get; set; }
	public string? SecondaryColour { get; set; }
}
