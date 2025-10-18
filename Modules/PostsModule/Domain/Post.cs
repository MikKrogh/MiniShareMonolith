
namespace PostsModule.Domain;

public class Post
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = "";
    public string? Description { get; private set; }
    public string CreatorName { get; private set; } = "";
    public string CreatorId { get; private set; } = "";
    public string FactionName { get; private set; } = "";
    public string? FigureName { get; private set; }
    public Colors PrimaryColor { get; private set; } = Colors.Unknown;
    public Colors SecondaryColor { get; private set; } = Colors.Unknown;
    public DateTime CreationDate { get; private set; } = default;
    public HashSet<string> Images { get; private set; } = new();

    private Post(string title, string creatorId, string factionName)
    {
        Title = title;
        CreatorId = creatorId;
        FactionName = factionName;
    }


    public static Post CreateNew(string title, string creatorId, string factionName, Guid? id = null)
    {
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(creatorId) || string.IsNullOrEmpty(factionName)) throw new ArgumentNullException("cant create posts with empty title or creatorId");

        var post = new Post(title, creatorId, factionName)
        {
            Id = id ?? Guid.NewGuid(),
            CreationDate = DateTime.UtcNow
        };
        return post;
    }
    public void SetId(Guid id)
    {
        if (id != Guid.Empty)
            Id = id;
    }
    public void SetId(string id)
    {
        if (Guid.TryParse(id, out var result))
            Id = result;
    }

    public void SetCreatorName(string creatorName)
    {
        if (!string.IsNullOrEmpty(creatorName))
            CreatorName = creatorName;
    }

    public void SetTitle(string title)
    {
        if (!string.IsNullOrEmpty(title))
            Title = title;
    }

    public void SetDescription(string? description)
    {
        Description = description;
    }

    public void SetPrimaryColor(string? color)
    {
        var result = Colors.Unknown;
        Enum.TryParse(color, true, out result);
        PrimaryColor = result;
    }
    public void SetSecondaryColor(string? color)
    {
        var result = Colors.Unknown;
        Enum.TryParse(color, true, out result);
        SecondaryColor = result;
    }
    public void SetCreationDate(DateTime creationDate)
    {
        if (creationDate != default)
            CreationDate = creationDate;
    }
}


