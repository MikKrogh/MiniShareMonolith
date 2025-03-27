using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UserModule.Tests")]
namespace UserModule;

internal class User
{
    public string Id { get; init; }
    public string UserName { get; init; }
    public DateTime CreationDate { get; init; }
}