using System.Net;

namespace PostsModule.Application.Create;

public record CreatePostResult
{
	public int ResultStatus { get; init; } = -1;
    public string? PostId { get; init; } = string.Empty;
	public bool IsSuccess { get; init; } = false;

    public CreatePostResult()    {}

	public static CreatePostResult Success(string postId)
	{
		if (string.IsNullOrEmpty(postId)) throw new Exception("cant have a successfull create post result without a postid");

		return new CreatePostResult
		{
			PostId = postId,
            IsSuccess = true,
            ResultStatus = (int)HttpStatusCode.OK
        };
	}
	public static CreatePostResult FailedToValidate() => new CreatePostResult
	{
		IsSuccess = false,
        ResultStatus = (int)HttpStatusCode.BadRequest
    };
    
}