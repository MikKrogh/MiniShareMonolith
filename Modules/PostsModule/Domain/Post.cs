namespace PostsModule.Domain;

public class Post
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public string Title { get; private set; } = "";
	public string? Description { get; private set; }
	public string CreatorName { get; private set; } = "";
	public string CreatorId { get; private set; } = "";
	public Colours PrimaryColour {  get; private set; } = Colours.Unknown;
	public Colours SecondaryColour { get; private set; } = Colours.Unknown;

	public Post(string title, string creatorId)
	{
		if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(creatorId))
			throw new ArgumentNullException("cant create posts with empty title or creatorId");
		Title = title;
		CreatorId = creatorId;
	}

	public void SetDescription(string? description)
	{
		Description = description;
	}

	public void SetDisplayName(string displayName)
	{
		if (!string.IsNullOrEmpty(displayName))		
			CreatorName = displayName;		
	}

	public void SetPrimaryColour(string? colour)
	{
		var result = Colours.Unknown;
		Enum.TryParse(colour, true, out result);
		PrimaryColour = result;
	}
	public void SetSecondaryColour(string? colour)
	{
		var result = Colours.Unknown;
		Enum.TryParse(colour, true, out result);
		SecondaryColour = result;
	}
}


