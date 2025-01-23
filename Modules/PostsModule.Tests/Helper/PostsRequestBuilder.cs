namespace PostsModule.Tests.Helper;

internal class PostsRequestBuilder
{
	private PostRequest postRequest;
	public PostsRequestBuilder Create()
	{
		postRequest = new PostRequest
		{
			CreatorId ="mikelong",
		};
		return this;
	}

	public PostsRequestBuilder WithTitle(string title)
	{
		postRequest.Title = title;
		return this;
	}
	public PostsRequestBuilder WithFactionName(string faction)
	{
		postRequest.FactionName = faction;
		return this;
	}
	public PostsRequestBuilder WithDescription(string description)
	{
		postRequest.Description = description;
		return this;
	}
	public PostsRequestBuilder WithMainColor(string colour)
	{
		postRequest.PrimaryColor = colour;
		return this;
	}
	public PostsRequestBuilder WithSecondaryColor(string colour)
	{
		postRequest.SecondaryColor = colour;
		return this;
	}

	public PostRequest Build() => postRequest;
}
