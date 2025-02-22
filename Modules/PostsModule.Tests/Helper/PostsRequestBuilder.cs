using Microsoft.AspNetCore.Http;

namespace PostsModule.Tests.Helper;

internal class PostsRequestBuilder
{
	private PostRequest postRequest;
	public PostsRequestBuilder Create(Guid? userID = null)
	{
		postRequest = new PostRequest
		{
			CreatorId = (userID == null) ? Guid.NewGuid().ToString() : userID.ToString(),
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

	public PostsRequestBuilder WithFile(IFormFile file)
	{
		if (postRequest.Images is null)
		{
			var filesCollection = new FormFileCollection();
			filesCollection.Add(file);
			postRequest.Images = filesCollection;
		}
		else
		{
			var updatedCollection = new FormFileCollection();
            updatedCollection.AddRange(postRequest.Images);
			updatedCollection.Add(file);
			postRequest.Images = updatedCollection;            
        }
		return this;
	}

    public PostRequest Build() => postRequest;
}
