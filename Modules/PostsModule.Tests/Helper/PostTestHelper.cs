using System.Net.Http.Headers;
using System.Text;

namespace PostsModule.Tests.Helper;

internal static class PostTestHelper
{
	public static ByteArrayContent GetFormFile(string fileName)
	{
		var fakeImageBytes = Encoding.UTF8.GetBytes("fake image content");
		var fileContent = new ByteArrayContent(fakeImageBytes);
		fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
		return fileContent;
	}
	public static List<ByteArrayContent> GetFormFiles(int amount)
	{
		var list = new List<ByteArrayContent>();

		for (int i = 0; i < amount; i++)
		{
			var fakeImageBytes = Encoding.UTF8.GetBytes("fake image content");
			var fileContent = new ByteArrayContent(fakeImageBytes);
			fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
			list.Add(fileContent);
		}
		return list;
	}

	public static PostRequest GetValidDefaultRequest()
	{
		return new PostsRequestBuilder().Create()
		.WithTitle("title")
		.WithFactionName("deathguard")
		.WithDescription("hello There")
		.WithMainColor("red")
		.WithSecondaryColor("blue")
		.Build();
	}


	public static MultipartFormDataContent CreateMultipartFormData(PostRequest request, IEnumerable<ByteArrayContent> filesContent)	{
		var formData = new MultipartFormDataContent();

		int counter = 0;
		foreach (var file in filesContent)
		{
			formData.Add(file, "file", $"filename{counter}.jpeg");
		}


		// Add JSON body as form fields (if it's not serialized, it can be done here)
		//var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonBody);
		var jsonObject = System.Text.Json.JsonSerializer.Serialize< PostRequest>(request);

		formData.Add(new StringContent(jsonObject));

		return formData;
	}
}
