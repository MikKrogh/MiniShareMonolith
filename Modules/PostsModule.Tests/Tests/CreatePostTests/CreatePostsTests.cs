using PostsModule.Tests.GetPostsTests;
using PostsModule.Tests.Helper;
using System.Net;

namespace PostsModule.Tests.CreatePostTests;

public class CreatePostsTests : IClassFixture<PostsWebApplicationFactory>
{
    private readonly TestFacade testFacade;
    public CreatePostsTests(PostsWebApplicationFactory factory)
    {
        testFacade = new TestFacade(factory);

    }

    [Fact]
    internal async Task GivenUserExists_WhenUserCreatesPost_ThenSuccessIsReturned()
    {
        // Given
        //await testFacade._factory.ThrowIfAzuriteNotRunning();
        var user = await testFacade.SendCreateUserEvent();

        //When
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        var create = await testFacade.SendCreatePost(createBody);


        //Then
        Assert.True(create.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    internal async Task GivenUserExists_WhenUserCreatesPost_ThenNewPostIdIsReturned()
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();

        //When
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        var create = await testFacade.SendCreatePost(createBody);

        //Then
        Assert.True(create.Result?.PostId != null);
    }

    [Fact]
    public async Task GivenNoUserExist_WhenSomeoneCreatesPost_ThenInternalServerErrorIsReturned()
    {
        // When
        var createBody = PostRequestBuilder.GetValidDefaultRequest(Guid.NewGuid().ToString());
        var create = await testFacade.SendCreatePost(createBody);

        //Then
        Assert.True(create.StatusCode == HttpStatusCode.InternalServerError);
    }


    [Theory,]
    [InlineData("")]
    [InlineData("Abclkjsd")]
    [InlineData("12365487654")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("ghfd777t-dfgh-987g-hgc6-gvjhg8976ygh")]
    [InlineData(null)]

    public async Task WhenSomeoneCreatesPostWithNonValidCreatorId_ThenBadRequestIsReturned(string? creatorId)
    {
        //When
        var createBody = PostRequestBuilder.GetValidDefaultRequest();
        createBody.CreatorId = creatorId;
        var response = await testFacade.SendCreatePost(createBody);

        //Then
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("min")]
    [InlineData("beyondMaxCharCountofThirtytwoXxXx")]
    [InlineData("stringWithNumeric5")]
    [InlineData("12365487654")]
    public async Task Given_UserExists_WhenSomeoneCreatesPostWithNonValidTitle_ThenBadRequestIsReturned(string? title)
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();

        //When
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        createBody.Title = title;
        var response = await testFacade.SendCreatePost(createBody);

        //Then
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Status code was: {response.StatusCode}");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("min")]
    [InlineData("stringWithNumeric5")]
    [InlineData("maxcountof25reachedXxXxXxX")]

    public async Task GivenUserExists_WhenSomeoneCreatesPostWithNonValidFaction_ThenBadRequestIsReturned(string? factionName)
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();

        //When
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        createBody.FactionName = factionName;
        var response = await testFacade.SendCreatePost(createBody);

        //Then
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Status code was: {response.StatusCode}");


    }

    [Theory]
    [InlineData("rEd")]
    [InlineData("reD")]
    [InlineData("RED")]
    [InlineData("red")]
    [InlineData("BLue")]
    [InlineData("BluE")]
    public async Task GivenUserExists_WhenUserCreatesPostWithWierdCasingForPrimaryColor_ThenSuccessIsReturnedAndColorIsSavedWithNormalName(string primaryColor)
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();

        //When
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        createBody.PrimaryColor = primaryColor;
        var response = await testFacade.SendCreatePost(createBody);

        //Then
        var getResponse = await testFacade.GetPost(response.Result.PostId);

        Assert.True(response.StatusCode == HttpStatusCode.OK);
        Assert.True(char.IsUpper(getResponse.PrimaryColor.ToString()[0]));
        Assert.True(getResponse.PrimaryColor.ToString().Skip(1).All(char.IsLower));
    }

    [Theory]
    [InlineData("rEd")]
    [InlineData("reD")]
    [InlineData("RED")]
    [InlineData("red")]
    [InlineData("BLue")]
    [InlineData("BluE")]
    public async Task GivenUserExists_WhenUserCreatesPostWithWierdCasingForSecondaryColor_ThenSuccessIsReturnedAndColorIsSavedWithNormalName(string secondaryColor)
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();

        //When
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        createBody.SecondaryColor = secondaryColor;
        var response = await testFacade.SendCreatePost(createBody);

        //Then
        var getResponse = await testFacade.GetPost(response.Result.PostId);

        Assert.True(response.StatusCode == HttpStatusCode.OK);
        Assert.True(char.IsUpper(getResponse.SecondaryColor.ToString()[0]));
        Assert.True(getResponse.SecondaryColor.ToString().Skip(1).All(char.IsLower));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("bogus")]
    [InlineData("r3d")]

    public async Task GivenUserExists_WhenSomeoneCreatesPostWithNonValidPrimaryColor_ThenSuccessIsReturnedAndPostIsSavedWithUndefinedPrimaryColor(string? primaryColor)
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();

        //When
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        createBody.PrimaryColor = primaryColor;
        var response = await testFacade.SendCreatePost(createBody);

        //Then
        var getResponse = await testFacade.GetPost(response.Result.PostId);

        Assert.True(Domain.Colors.Unknown == getResponse.PrimaryColor);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("bogus")]
    [InlineData("r3d")]
    public async Task GivenUserExists_WhenSomeoneCreatesPostWithNonValidSecondaryColor_ThenSuccessIsReturnedAndPostIsSavedWithUndefinedPrimaryColor(string? secondaryColor)
    {
        // Given
        var user = await testFacade.SendCreateUserEvent();

        //When
        var createBody = PostRequestBuilder.GetValidDefaultRequest(user.UserId);
        createBody.SecondaryColor = secondaryColor;
        var response = await testFacade.SendCreatePost(createBody);

        //Then
        var getResponse = await testFacade.GetPost(response.Result.PostId);

        Assert.True(Domain.Colors.Unknown == getResponse.SecondaryColor);
    }
}
