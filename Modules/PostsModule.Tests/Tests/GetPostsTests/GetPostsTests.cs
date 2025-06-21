using PostsModule.Application;
using PostsModule.Tests.Helper;
using System.Net;

namespace PostsModule.Tests.GetPostsTests;
[Collection(nameof(SystemTestCollectionDefinition))]

public class GetPostsTests : IClassFixture<PostsWebApplicationFactory>
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
        var createBody = PostRequestBuilder.GetValidDefaultBody();
        var firstCreation = await testFacade.SendCreatePost(createBody, user.UserId);
        var secondCreation = await testFacade.SendCreatePost(createBody, user.UserId);

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

        var firstPost = new PostRequestBuilder().Create()
            .WithTitle("first")
            .WithFactionName("firstfaction")
            .WithDescription("firstdescp")
            .WithMainColor("red")
            .WithSecondaryColor("blue")
        .Build();

        var secondPost = new PostRequestBuilder().Create()
            .WithTitle("second")
            .WithFactionName("seoncdfaction")
            .WithDescription("seconddescp")
            .WithMainColor("yellow")
            .WithSecondaryColor("green")
        .Build();

        await testFacade.SendCreatePost(firstPost, user.UserId);
        await testFacade.SendCreatePost(secondPost, user.UserId);

        // When        
        var getPosts = await testFacade.GetPosts();

        // Then
        Assert.Equal(HttpStatusCode.OK, getPosts.StatusCode);
        Assert.NotEmpty(getPosts.Result.Items);
        Assert.Equal(2, getPosts.Result.Items.Count());

        var firstDto = getPosts.Result.Items.First(x => x.Title == firstPost.Title);
        var secondDto = getPosts.Result.Items.First(x => x.Title == secondPost.Title);


        Assert.True(RequestMatchesDto(firstDto, firstPost, user.UserId));
        Assert.True(RequestMatchesDto(secondDto, secondPost, user.UserId));

    }

    [Fact]
    public async Task GivenOnlyTwoPostsExistsAndUserHasUploadedImageToEach_WhenUserAsksForPosts_ThenImagePathsAreSet()
    {
        // Given
        testFacade.TruncateTables();
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultBody();
        var firstCreation = await testFacade.SendCreatePost(createBody, user.UserId);
        var secondCreation = await testFacade.SendCreatePost(createBody, user.UserId);

        await testFacade.UploadImage(firstCreation.Result.PostId, firstCreation.Result.Token);
        await testFacade.UploadImage(secondCreation.Result.PostId, secondCreation.Result.Token);

        // When
        var getPosts = await testFacade.GetPosts();


        // Then 
        Assert.Equal(2, getPosts.Result?.Items.Count());

        var firstPost = getPosts.Result.Items.SingleOrDefault(x => x.Id == firstCreation.Result.PostId);
        var secondPost = getPosts.Result.Items.SingleOrDefault(x => x.Id == secondCreation.Result.PostId);
        Assert.Equal(1, firstPost?.Images?.Count());
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
        var createBody = PostRequestBuilder.GetValidDefaultBody();
        var firstCreation = await testFacade.SendCreatePost(createBody, user.UserId);
        var secondCreation = await testFacade.SendCreatePost(createBody, user.UserId);
        var thirdCreation = await testFacade.SendCreatePost(createBody, user.UserId);

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

    [Fact(Skip = "Only run manually")]
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
        await CreatePosts(5, true);

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
        await CreatePosts(5, true);

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
        var defaultPost = PostRequestBuilder.GetValidDefaultBody();
        await testFacade.SendCreatePost(defaultPost, user.UserId);
        await testFacade.SendCreatePost(defaultPost, user.UserId);
        await testFacade.SendCreatePost(defaultPost with { PrimaryColor = "red" }, user.UserId);

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
        var defaultPost = PostRequestBuilder.GetValidDefaultBody();
        await testFacade.SendCreatePost(defaultPost, user.UserId);
        await testFacade.SendCreatePost(defaultPost, user.UserId);
        await testFacade.SendCreatePost(defaultPost with { SecondaryColor = "red" }, user.UserId);

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
        var postByUserOne = PostRequestBuilder.GetValidDefaultBody();
        await testFacade.SendCreatePost(postByUserOne, userOne.UserId);

        var userTwo = await testFacade.SendCreateUserEvent();
        var postByUserTwo = PostRequestBuilder.GetValidDefaultBody();
        await testFacade.SendCreatePost(postByUserTwo, userTwo.UserId);
        await testFacade.SendCreatePost(postByUserTwo, userTwo.UserId);


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
        var post = PostRequestBuilder.GetValidDefaultBody();
        post.FactionName = "default";
        await testFacade.SendCreatePost(post, user.UserId);

        var updatedPost = post with { FactionName = "spaceMarines" };
        await testFacade.SendCreatePost(updatedPost, user.UserId);
        await testFacade.SendCreatePost(updatedPost, user.UserId);

        // When
        var getPosts = await testFacade.GetPosts($"filter=Faction eq '{updatedPost.FactionName}'");


        // Then
        Assert.NotNull(getPosts.Result);
        Assert.Equal(2, getPosts.Result.Items.Count());
        Assert.Equal(2, getPosts.Result.TotalCount);
        Assert.All(getPosts.Result.Items, x => Assert.Equal(updatedPost.FactionName, x.FactionName));
    }

    [Fact]
    public async Task GivenThreePostsExistsAndOneHasFactionWithSpaceSeperatorInItsName_WhenUserQueriesForThisFaction_ThenMatchingPostIsReturnedAndTotalCountIsOne()
    {
        // Given
        testFacade.TruncateTables();
        var user = await testFacade.SendCreateUserEvent();
        var post = PostRequestBuilder.GetValidDefaultBody();        
        await testFacade.SendCreatePost(post, user.UserId);
        await testFacade.SendCreatePost(post, user.UserId);

        var updatedPost = post with { FactionName = "space marines" };        
        await testFacade.SendCreatePost(updatedPost, user.UserId);

        // When
        var getPosts = await testFacade.GetPosts($"filter=Faction eq '{updatedPost.FactionName}'");

        // Then
        Assert.NotNull(getPosts.Result);
        Assert.Single(getPosts.Result.Items);
        Assert.Equal(1, getPosts.Result.TotalCount);
        Assert.All(getPosts.Result.Items, x => Assert.Equal(updatedPost.FactionName, x.FactionName));
    }


    [Fact]
    public async Task GivenThreePostsExistsAllWithTheSamePrimaryColorAndTwopostsShareSecondaryColor_WhenUserQueriesThePrimaryAndSecondaryColor_ThenTwoPostsAreTurnedWithTheSecondayColor()
    {
        // When
        testFacade.TruncateTables();

        var user = await testFacade.SendCreateUserEvent();
        var post = PostRequestBuilder.GetValidDefaultBody();
        post.PrimaryColor = "red";
        post.SecondaryColor = "yellow";
        await testFacade.SendCreatePost(post, user.UserId);

        var updatedPost = post with { SecondaryColor = "blue" };
        await testFacade.SendCreatePost(updatedPost, user.UserId);
        await testFacade.SendCreatePost(updatedPost, user.UserId);

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
        // Given
        testFacade.TruncateTables();

        var user = await testFacade.SendCreateUserEvent();
        var post = PostRequestBuilder.GetValidDefaultBody();
        await testFacade.SendCreatePost(post, user.UserId);
        await testFacade.SendCreatePost(post, user.UserId);
        var postWithUniqueTitle = post with { Title = "UniqueTitle" };
        await testFacade.SendCreatePost(postWithUniqueTitle, user.UserId);

        // When
        var getPosts = await testFacade.GetPosts($"search={postWithUniqueTitle.Title}");

        // Then
        Assert.NotNull(getPosts.Result);
        Assert.Single(getPosts.Result.Items);
        Assert.Equal(1, getPosts.Result.TotalCount);
        Assert.Equal(postWithUniqueTitle.Title, getPosts.Result.Items.Single().Title);
    }


    [Fact]
    public async Task GivenThreePostsExistsAndOneHasAUniqueDescription_WhenUserQueriesWithASearchForThisDescription_ThenMatchingPostIsReturnedWithCorrectTotalCount()
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();
        var post = PostRequestBuilder.GetValidDefaultBody();
        await testFacade.SendCreatePost(post, user.UserId);
        await testFacade.SendCreatePost(post, user.UserId);
        var postWithUniqueDescription = post with { Description = "UniqueDescription" };
        await testFacade.SendCreatePost(postWithUniqueDescription, user.UserId);

        // When
        var getPosts = await testFacade.GetPosts($"search={postWithUniqueDescription.Description}");

        // Then
        Assert.NotNull(getPosts.Result);
        Assert.Single(getPosts.Result.Items);
        Assert.Equal(1, getPosts.Result.TotalCount);
        Assert.Equal(postWithUniqueDescription.Description, getPosts.Result.Items.Single().Description);
    }

    [Fact(Skip = "Only run manually")]
    public async Task Given150PostsExistsAnd110OfThemHaveMatchingTitleAndRedPrimaryColorWhenUserQueriesForRedPrimaryColor_Thern100PostsAreReturnedAndTotalCountIs110()
    {
        // Given
        await CreatePosts(20);

        var user = await testFacade.SendCreateUserEvent();
        var post = PostRequestBuilder.GetValidDefaultBody();
        post.PrimaryColor = "Red";
        post.Title = "unique";
        var postsMatchingQuery = Enumerable.Range(0, 110).Select(x => testFacade.SendCreatePost(post, user.UserId));
        await Task.WhenAll(postsMatchingQuery);

        await CreatePosts(20);

        // When
        var getPosts = await testFacade.GetPosts($"search={post.Title}&filter=PrimaryColor eq '{post.PrimaryColor}'&orderBy=CreationDate desc");

        // Then
        Assert.NotNull(getPosts.Result);
        Assert.Equal(100, getPosts.Result.Items.Count());
        Assert.Equal(110, getPosts.Result.TotalCount);
        Assert.All(getPosts.Result.Items, x => Assert.Equal(post.PrimaryColor.ToLower(), x.PrimaryColor.ToString().ToLower()));
        Assert.All(getPosts.Result.Items, x => Assert.Equal(post.Title, x.Title));
    }


    [Fact]
    public async Task Given20PostsExistsAndUserHasAlreadyQueried10_WhenUserQueriesWithATakeOf10AndSkip10_Then10DifferentPostsAreReturned()
    {
        // Given
        await CreatePosts(20);
        var firstQuery = await testFacade.GetPosts("take=10");

        //When
        var secondQuery = await testFacade.GetPosts("take=10&skip=10");

        // Then
        Assert.NotNull(firstQuery.Result);
        Assert.NotNull(secondQuery.Result);
        var allPosts = firstQuery.Result.Items.ToList();
        allPosts.AddRange(secondQuery.Result.Items.ToList());
        Assert.True(allPosts.Select(x => x.Id).Distinct().Count() == allPosts.Count());
    }


    [Fact]
    public async Task Given20PostsExistsAndUserHasAlreadyQueried10WithOrderByCreationTime_WhenUserQueriesWithATakeOf10AndSkip10AndOrderByTime_AllPostsAreSortedCorrectly()
    {
        // Given
        await CreatePosts(20, true);
        var firstQuery = await testFacade.GetPosts("take=10&orderBy=CreationDate desc");

        //When
        var secondQuery = await testFacade.GetPosts("take=10&skip=10orderBy=CreationDate desc");

        // Then
        Assert.NotNull(firstQuery.Result);
        Assert.NotNull(secondQuery.Result);
        var allPosts = firstQuery.Result.Items.ToList();
        allPosts.AddRange(secondQuery.Result.Items.ToList());
        Assert.True(allPosts.Select(x => x.Id).Distinct().Count() == allPosts.Count());

        var expetedSortOrder = allPosts.OrderByDescending(x => x.CreationDate);
        Assert.Equal(expetedSortOrder, allPosts);

    }



    //allTheAbove
    private bool RequestMatchesDto(PostDto dto, PostRequest request, string userId)
    {
        return dto.Title == request.Title
            && dto.CreatorId == userId
            && dto.FactionName == request.FactionName
            && dto.Description == request.Description
            && dto.PrimaryColor.ToString().ToLower() == request.PrimaryColor
            && dto.SecondaryColor.ToString().ToLower() == request.SecondaryColor;
    }

    private async Task CreatePosts(int count, bool introduceDelay = false)
    {
        var user = await testFacade.SendCreateUserEvent();
        var createBody = PostRequestBuilder.GetValidDefaultBody();

        for (int i = 0; i < count; i++)
        {
            if (introduceDelay)
                await Task.Delay(50); //make it easier to verify that order by date is readable            
            await testFacade.SendCreatePost(createBody, user.UserId);

        }
    }
}

//becourse these test assert on collections of all items in the db, its important not to run in parralel with other tests.
[CollectionDefinition(nameof(SystemTestCollectionDefinition), DisableParallelization = true)]
public class SystemTestCollectionDefinition { }