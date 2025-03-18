using PostsModule.Application;
using PostsModule.Domain;
using PostsModule.Tests.Helper;
using System.Net;

namespace PostsModule.Tests.Tests.GetPostsTests;
[Collection(nameof(SystemTestCollectionDefinition))]
public class GetPostsTests: IClassFixture<PostsWebApplicationFactory>
{
    private readonly TestFacade testFacade;
    public GetPostsTests(PostsWebApplicationFactory factory)
    {        
        testFacade = new TestFacade(factory);
    }

    
    [Fact]
    internal async Task GivenMultiplePostsExists_WhenUserAsksForPosts_ThenSuccessIsReturned()
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        var firstCreation = await testFacade.SendCreatePost(createBody);
        var secondCreation = await testFacade.SendCreatePost(createBody);
        
        // When        
        var posts = await testFacade.GetPosts();

        // Then
        Assert.Equal(HttpStatusCode.OK, posts.StatusCode);
    }

    [Fact]
    internal async Task GivenOnlyTwoPostsExists_WhenUserAsksForPosts_ThenCorrectValuesAreReturned()
    {
        testFacade.TruncateTables();
        var user = await testFacade.SendCreateUserEvent();

        var firstPost = new PostRequestBuilder().Create(user.UserId)
            .WithTitle("first")
            .WithFactionName("firstfaction")
            .WithDescription("firstdescp")
            .WithMainColor("red")
            .WithSecondaryColor("blue")
        .Build();

        var secondPost = new PostRequestBuilder().Create(user.UserId)
            .WithTitle("second")
            .WithFactionName("seoncdfaction")
            .WithDescription("seconddescp")
            .WithMainColor("yellow")
            .WithSecondaryColor("green")
        .Build();

        await testFacade.SendCreatePost(firstPost);
        await testFacade.SendCreatePost(secondPost);

        // When        
        var getPosts = await testFacade.GetPosts();

        // Then
        Assert.Equal(HttpStatusCode.OK, getPosts.StatusCode);
        Assert.NotEmpty(getPosts.Result.Items);
        Assert.Equal(2, getPosts.Result.Items.Count());
            
        var firstDto = getPosts.Result.Items.First(x => x.Title == firstPost.Title);
        var secondDto = getPosts.Result.Items.First(x => x.Title == secondPost.Title);         
            

        Assert.True(RequestMatchesDto(firstDto, firstPost));
        Assert.True(RequestMatchesDto(secondDto, secondPost));
        
    }

    [Fact]
    public async Task GivenOnlyTwoPostsExistsAndUserHasUploadedImageToEach_WhenUserAsksForPosts_ThenImagePathsAreSet()
    {
        // Given
        testFacade.TruncateTables();
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        var firstCreation = await testFacade.SendCreatePost(createBody);
        var secondCreation = await testFacade.SendCreatePost(createBody);

        await testFacade.UploadImage(firstCreation.Result.PostId, firstCreation.Result.Token);
        await testFacade.UploadImage(secondCreation.Result.PostId, secondCreation.Result.Token);

        // When
        var getPosts = await testFacade.GetPosts();


        // Then 
        Assert.Equal(2, getPosts.Result?.Items.Count());

        var firstPost = getPosts.Result.Items.SingleOrDefault(x => x.Id == firstCreation.Result.PostId);
        var secondPost = getPosts.Result.Items.SingleOrDefault(x => x.Id == secondCreation.Result.PostId);
        Assert.Equal(1,firstPost?.Images?.Count());
        Assert.Equal(1, secondPost?.Images?.Count());
        Assert.NotEqual(firstPost?.Images?.Single(), secondPost?.Images?.Single());

    }

    //pagination
    [Fact]
    public async Task GivenThreePostsExists_WhenUserQueriesPosts_ThenTotalCountIsReturned()
    {
        // Given
        testFacade.TruncateTables();
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        var firstCreation = await testFacade.SendCreatePost(createBody);
        var secondCreation = await testFacade.SendCreatePost(createBody);
        var thirdCreation = await testFacade.SendCreatePost(createBody);

        // When
        var getPosts = await testFacade.GetPosts();

        Assert.Equal(3, getPosts.Result.TotalCount);
    }

    [Fact]
    public async Task GivenThreePostsExist_WhenUserQueriesWithATakeOfOne_ThenOnePostIsReturnedAndTotalCountIsThree()
    {
        // Given
        testFacade.TruncateTables();
        await CreatePosts(3);

        // When
        var getPosts = await testFacade.GetPosts("take=1");

        // Then
        Assert.NotNull(getPosts.Result);
        Assert.Single(getPosts.Result.Items);
        Assert.Equal(3, getPosts.Result.TotalCount);
    }

    [Fact]
    public async Task GivenOnehundredandTenPostsExists_WhenUserQuriesWithATakerMoreThanHundred_ThenOnlyAHundredAreReturned()
    {
        // Given
        int totalPosts = 110;
        testFacade.TruncateTables();
        await CreatePosts(totalPosts);

        // When
        var getPosts = await testFacade.GetPosts("take=150");

        // Then
        Assert.NotNull(getPosts.Result);
        Assert.Equal(100, getPosts.Result.Items.Count());
        Assert.Equal(totalPosts, getPosts.Result.TotalCount);
    }

    [Fact]
    public async Task GivenFivePostExists_WhenUserQueriesWithOrderByNewest_ThenReturnedPostsAreOrderedByNewestCreationDate()
    {

        // Given
        testFacade.TruncateTables();
        await CreatePosts(5);

        // When
        var getPost = await testFacade.GetPosts("orderBy=CreationDate desc");

        //Then
        Assert.NotNull(getPost.Result);
        Assert.True(getPost.Result.Items.SequenceEqual(getPost.Result.Items.OrderByDescending(i => i.CreationDate)),
        "Collection is not sorted in descending order.");
    }
    [Fact]
    public async Task GivenFivePostExists_WhenUserQueriesWithOrderByOldest_ThenReturnedPostsAreOrderedByOldetCreationDate()
    {
        // Given
        testFacade.TruncateTables();
        await CreatePosts(5);

        // When
        var getPost = await testFacade.GetPosts("orderBy=CreationDate");

        //Then
        Assert.NotNull(getPost.Result);
        Assert.True(getPost.Result.Items.SequenceEqual(getPost.Result.Items.OrderBy(i => i.CreationDate)),
        "Collection is not sorted in descending order.");
    }

    [Fact]
    public async Task GivenThreePostsExistsAndOneHasRedPrimaryColor_WhenUserFiltersByRedPrimaryColor_ThenOnePostIsReturnedAndTotalCountIsOne()
    {
        // Given
        testFacade.TruncateTables();
        var user = await testFacade.SendCreateUserEvent();
        var defaultPost = PostRequestBuilder.GetValidDefaultRequest(user.UserId); 
        await testFacade.SendCreatePost(defaultPost);
        await testFacade.SendCreatePost(defaultPost);
        await testFacade.SendCreatePost(defaultPost with { PrimaryColor = "red" });

        // When
        var getPosts = await testFacade.GetPosts("filter=PrimaryColor eq 'red'");

        // Then
        Assert.NotNull(getPosts.Result);
        Assert.Single(getPosts.Result.Items);
        Assert.Equal(1, getPosts.Result.TotalCount);
        Assert.Equal("red", getPosts.Result.Items.Single().PrimaryColor.ToString().ToLower());
        
    }

    [Fact]
    public async Task GivenThreePostsExistsAndOneHasRedSecondaryColor_WhenUserFiltersByRedSecondaryColor_ThenOnePostIsReturnedAndTotalCountIsOne()
    {
        // Given
        testFacade.TruncateTables();
        var user = await testFacade.SendCreateUserEvent();
        var defaultPost = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        await testFacade.SendCreatePost(defaultPost);
        await testFacade.SendCreatePost(defaultPost);
        await testFacade.SendCreatePost(defaultPost with { SecondaryColor = "red" });

        // When
        var getPosts = await testFacade.GetPosts("filter=SecondaryColor eq 'red'");

        // Then
        Assert.NotNull(getPosts.Result);
        Assert.Single(getPosts.Result.Items);
        Assert.Equal(1, getPosts.Result.TotalCount);
        Assert.Equal("red", getPosts.Result.Items.Single().SecondaryColor.ToString().ToLower());
    }

    [Fact]
    public async Task GivenThreePostsExistsAndTwoAreCreatedByTheSamePerson_WhenUserQueriesByThisPersonsId_ThenTwoPostsAreReturnedAndTotalCountIsTwo()
    {
        // Given
        testFacade.TruncateTables();

        var userOne = await testFacade.SendCreateUserEvent();
        var postByUserOne = PostRequestBuilder.GetValidDefaultRequest(userOne.UserId);
        await testFacade.SendCreatePost(postByUserOne);

        var userTwo = await testFacade.SendCreateUserEvent();
        var postByUserTwo = PostRequestBuilder.GetValidDefaultRequest(userTwo.UserId);        
        await testFacade.SendCreatePost(postByUserTwo);
        await testFacade.SendCreatePost(postByUserTwo);


        // When
        var getPosts = await testFacade.GetPosts($"filter=CreatorId eq '{userTwo.UserId}'");

        // Then
        Assert.NotNull(getPosts.Result);
        Assert.Equal(2, getPosts.Result.Items.Count());
        Assert.Equal(2, getPosts.Result.TotalCount);
        Assert.All(getPosts.Result.Items, x => Assert.Equal(userTwo.UserId.ToString(), x.CreatorId));
    }

    [Fact]
    public async Task GivenThreePostsExistsWhereTwoHaveTheSameFaction_WhenUserQueriesForThatFaction_ThenMatchingPostAreReturnedAndTotalCountIsTwo()
    {
        // Given
        testFacade.TruncateTables();

        var user = await testFacade.SendCreateUserEvent();
        var post = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        post.FactionName = "default";
        await testFacade.SendCreatePost(post);

        var updatedPost = post with { FactionName = "spaceMarines" };
        await testFacade.SendCreatePost(updatedPost);
        await testFacade.SendCreatePost(updatedPost);

        // When
        var getPosts = await testFacade.GetPosts($"filter=Faction eq '{updatedPost.FactionName}'");


        // Then
        Assert.NotNull(getPosts.Result);
        Assert.Equal(2, getPosts.Result.Items.Count());
        Assert.Equal(2, getPosts.Result.TotalCount);
        Assert.All(getPosts.Result.Items, x => Assert.Equal(updatedPost.FactionName, x.FactionName));
    }


    [Fact]
    public async Task GivenThreePostsExistsAllWithTheSamePrimaryColorAndTwopostsShareSecondaryColor_WhenUserQueriesThePrimaryAndSecondaryColor_ThenTwoPostsAreTurnedWithTheSecondayColor()
    {
        // When
        testFacade.TruncateTables();

        var user = await testFacade.SendCreateUserEvent(); 
        var post = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        post.PrimaryColor = "red";
        post.SecondaryColor = "yellow";
        await testFacade.SendCreatePost(post);

        var updatedPost = post with { SecondaryColor = "blue" };
        await testFacade.SendCreatePost(updatedPost);
        await testFacade.SendCreatePost(updatedPost);

        // When
        var getPosts = await testFacade.GetPosts($"filter=PrimaryColor eq '{post.PrimaryColor}' AND SecondaryColor eq '{updatedPost.SecondaryColor}'");

        // Then
        Assert.NotNull(getPosts.Result);
        Assert.Equal(2, getPosts.Result.Items.Count());
        Assert.Equal(2, getPosts.Result.TotalCount);
        Assert.All(getPosts.Result.Items, x => Assert.Equal(updatedPost.SecondaryColor.ToLower(), x.SecondaryColor.ToString().ToLower()));
    }

    [Fact]
    public async Task GivenThreePostsExistsAndOneHasAUniqueTitle_WhenUserQueriesWithASearchForThisTitle_ThenMatchingPostIsReturnedWithCorrectTotalCount()
    {

    }



    //pagination

    //allTheAbove
    private bool RequestMatchesDto(PostDto dto, PostRequest request)
    {

        return dto.Title == request.Title
            && dto.CreatorId == request.CreatorId
            && dto.FactionName == request.FactionName
            && dto.Description == request.Description
            && dto.PrimaryColor.ToString().ToLower() == request.PrimaryColor
            && dto.SecondaryColor.ToString().ToLower() == request.SecondaryColor;
    }

    private async Task CreatePosts(int count)
    {
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);

        for (int i = 0; i < count; i++)
        {
            await Task.Delay(50); //make it easier to verify that order by date is readable
            await testFacade.SendCreatePost(createBody);

        }
    }
}

//becourse these test depend certain entites, its important not to run in paralel, and instead perform cleanup in the tables
[CollectionDefinition(nameof(SystemTestCollectionDefinition), DisableParallelization = true)]
public class SystemTestCollectionDefinition { }