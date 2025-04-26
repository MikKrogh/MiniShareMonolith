
namespace PostsModule.Tests.Helper;

internal class PostRequestBuilder
{
    private PostRequest postRequest;
    public PostRequestBuilder Create()
    {
        postRequest = new PostRequest();    
        return this;
    }


    public PostRequestBuilder WithTitle(string title)
    {
        postRequest.Title = title;
        return this;
    }
    public PostRequestBuilder WithFactionName(string faction)
    {
        postRequest.FactionName = faction;
        return this;
    }
    public PostRequestBuilder WithDescription(string description)
    {
        postRequest.Description = description;
        return this;
    }
    public PostRequestBuilder WithMainColor(string color)
    {
        postRequest.PrimaryColor = color;
        return this;
    }
    public PostRequestBuilder WithSecondaryColor(string color)
    {
        postRequest.SecondaryColor = color;
        return this;
    }
    public PostRequest Build() => postRequest;

    public static PostRequest GetValidDefaultBody()
    {
        return new PostRequestBuilder().Create()
        .WithTitle("title")
        .WithFactionName("deathguard")
        .WithDescription("hello There")
        .WithMainColor("unknown")
        .WithSecondaryColor("unknown")
        .Build();
    }
}
internal record PostRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string FactionName { get; set; }
    public string PrimaryColor { get; set; }
    public string SecondaryColor { get; set; }
}