using Microsoft.AspNetCore.Http;

namespace PostsModule.Tests.Helper;

internal class PostRequest
{
    public string CreatorId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string FactionName { get; set; }
    public string PrimaryColor { get; set; }
    public string SecondaryColor { get; set; }
    public IFormFileCollection Images { get; set; }
}
