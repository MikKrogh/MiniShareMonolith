namespace PostsModule.Domain;

public class User
{
    public string Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private User() { }

    public static User Create(string id)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentException("user cannot have empty id");
        return new User() { Id = id };
    }

    public void SetName(string name)
    {
        Name = name;
    }
}


