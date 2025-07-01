
using PostsModule.Tests.Helper;
using System.Net;


namespace PostsModule.Tests.Tests.DeletePostTests;

public class DeletePostsTests : IClassFixture<PostsWebApplicationFactory>
{
    private readonly TestFacade testFacade;
    public DeletePostsTests(PostsWebApplicationFactory factory)
    {
        ///<summary >
        // Deleting posts works with a background worker which runs a loop looking for posts to delete. 
        // the nature of the setup results in a few delays between the http call to delete and the actual deletion
        // if tests begin failing due to timing issues, increase the delay in the test, or decrease the loop time in the background worker.
        /// </summary>
        testFacade = new TestFacade(factory);
    }
    [Fact]
    public async Task GivenPostExists_WhenUserDeletesPost_ThenSuccessIsReturned() 
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();
        var post = await testFacade.SendCreatePost(PostRequestBuilder.GetValidDefaultBody(), user.UserId);
        // When
        var response = await testFacade.DeletePost(post.Result.PostId,user.UserId);
        // Then
        Assert.Equal(HttpStatusCode.OK,response);

    }
    [Fact]
    public async Task GivenPostExistsWithImages_WhenPostIsDeletedAndUserTriesToFetchPost_ThenNotFoundIsReturn()
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();
        var post = await testFacade.SendCreatePost(PostRequestBuilder.GetValidDefaultBody(), user.UserId);

        // When
        var response = await testFacade.DeletePost(post.Result.PostId, user.UserId);
        await Task.Delay(500);
        var notfound = await testFacade._client.GetAsync(post.Result.PostId);

        // Then
        Assert.Equal(HttpStatusCode.NotFound, notfound.StatusCode);
    }

    [Fact]
    public async Task GivenPostExists_WhenUserDeletesPost_ThenImagesAreDeleted()
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();
        var post = await testFacade.SendCreatePost(PostRequestBuilder.GetValidDefaultBody(), user.UserId);
        await testFacade.UploadImage(post.Result.PostId, post.Result.Token);
        await testFacade.UploadImage(post.Result.PostId, post.Result.Token);
        await testFacade.UploadThumbnail(post.Result.PostId, post.Result.Token);
        var postDto = await testFacade.GetPost(post.Result.PostId);
        // When
        var response = await testFacade.DeletePost(post.Result.PostId, user.UserId);
        await Task.Delay(500);

        // Then
        var imageOne = await testFacade.GetImage(postDto.Id, postDto.Images.ToList()[0]);
        var imageTwo = await testFacade.GetImage(postDto.Id, postDto.Images.ToList()[1]);
        var thumbnail = await testFacade.GetThumbnail(postDto.Id);

        Assert.Equal(HttpStatusCode.NotFound, imageOne.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, imageTwo.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, thumbnail.StatusCode);
    }

    [Fact]
    public async Task GivenPostExists_WhenUserDeletesPost_ThenPostDeletedEventIsPublished()
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();
        var post = await testFacade.SendCreatePost(PostRequestBuilder.GetValidDefaultBody(), user.UserId);
        // When
        var response = await testFacade.DeletePost(post.Result.PostId, user.UserId);
        // Then
        Assert.Equal(HttpStatusCode.OK, response);
        await testFacade.MessageBrokerTestFacade.WaitUntilEventHasBeenConsumed<PostDeletedEvent>(x => x.PostId == post.Result.PostId);
    }



}
