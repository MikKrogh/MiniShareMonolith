namespace PostsModule.Presentation.Endpoints;
internal class CreateBody
{
	public string? Title {  get; set; }
	public string? CreatorId { get; set; }
	public string? Description { get; set; }
	public string? PrimaryColour { get; set; }
	public string? SecondaryColour { get; set; }
}
