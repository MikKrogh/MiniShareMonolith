
using PostsModule.Presentation.Endpoints;
using PostsModule.Tests.Helper;
using System.Net;

namespace PostsModule.Tests.Tests.DeletePostTests;

public class DeletePostsTests : IClassFixture<PostsWebApplicationFactory>
{
    private readonly TestFacade testFacade;
    public DeletePostsTests(PostsWebApplicationFactory factory)
    {
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
    //givenPostexists_WhenPostExists_ThenPostIsDeleted()
    [Fact]
    public async Task GivenPostExistsWithImages_WhenPostisDeleted_ThenPostIsDeleted()
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();
        var post = await testFacade.SendCreatePost(PostRequestBuilder.GetValidDefaultBody(), user.UserId);

        await testFacade.UploadImage(post.Result.PostId, post.Result.Token);
        await testFacade.UploadImage(post.Result.PostId, post.Result.Token);
        await testFacade.UploadThumbnail(post.Result.PostId, post.Result.Token);


        // When
        var response = await testFacade.DeletePost(post.Result.PostId, user.UserId);
        
        // Then
        Assert.Equal(HttpStatusCode.OK, response);


    }
    //givenPostexists_WhenPostExists_ThenImagesAreDeleted()
    //given no post exists then 404 is returned
    //givenimagesExists_whenuserdeletes_thenimagesaredeleted
    //postdeletedEventIsPublished



}
