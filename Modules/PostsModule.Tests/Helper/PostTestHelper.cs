using Microsoft.AspNetCore.Http;
using System.Text;

namespace PostsModule.Tests.Helper;

internal static class PostTestHelper
{
    public static FormFile CreateFormFile(string fileName)
    {
        byte[] imageBytes = Encoding.UTF8.GetBytes("fake image content");
        string contentType = "image/jpeg";

        var stream = new MemoryStream(imageBytes);

        var formFile = new FormFile(stream, 0, imageBytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
        return formFile;
    }

    public static PostRequest GetValidDefaultRequest(Guid? userId = null)
	{
		return new PostsRequestBuilder().Create(userId)
		.WithTitle("title")
		.WithFactionName("deathguard")
		.WithDescription("hello There")
		.WithMainColor("red")
		.WithSecondaryColor("blue")
		.WithFile(CreateFormFile("fakeImage.jpeg"))
		.Build();
	}
}
