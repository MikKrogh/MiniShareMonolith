using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UserModule.Tests")]
namespace UserModule;

public class User
{
    public string Id { get; init; }
    public string UserName { get; init; }
    public DateTime CreationDate { get; init; }
}