using BarebonesMessageBroker;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using UserModule.Features.CreateUser;

namespace UserModule.Tests.Tests;

public class CreateUserTests : IClassFixture<UserWebApplicationFactory>
{
    HttpClient client;
    TestBus _busTestFacade;
    public CreateUserTests(UserWebApplicationFactory factory)
    {
        client = factory.CreateClient();
        var busInterFace = factory.Services.GetRequiredService<IBus>();
        _busTestFacade = busInterFace as TestBus ?? throw new Exception("couldnt find Testbus");
    }
    [Fact]
    public async Task WhenUserSignsUpWithValidInformaiton_ThenSuccessIsReturned()
    {
        // When
        var userToCreate = UserBuilder.GenerateUserToCreate();
        var response = await client.SendCreateUserRequest(userToCreate);

        // Then
        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode);
    }


    [Fact]
    public async Task WhenUserSignsUpWithValidInformaiton_ThenUserCreatedEventIsSent()
    {
        //When
       var userToCreate = UserBuilder.GenerateUserToCreate();
        await client.SendCreateUserRequest(userToCreate);

        // Then
        var wasPublished = _busTestFacade.WasPublished<UserCreatedEvent>("UserModule.UserCreated", x => x.UserName == userToCreate.DisplayName && x.UserId == userToCreate.UserId);

        Assert.True(wasPublished, "Expected event was not published in time");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task WhenUserIsCreatedWithNonValidId_ThemBadRequestIsReturnedAndNoEventIsSent(string? invalidId)
    {
        // When
        var requestBody = UserBuilder.GenerateUserToCreate();
        requestBody.UserId = invalidId;
        var response = await client.PostAsJsonAsync("User", requestBody);

        // Then

        await Task.Delay(100);
        var waspublished = _busTestFacade.WasPublished<UserCreatedEvent>("UserModule.UserCreated", x => x.UserName == requestBody.DisplayName && x.UserId == requestBody.UserId);


        Assert.False(waspublished);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GivenOneUserExists_WhenSomeoneSignsupWithTheSameUserName_ThenBadRequestIsReturnedAndNoEventIsSent()
    {
        //Given 
        var initialUser = UserBuilder.GenerateUserToCreate();
         await client.SendCreateUserRequest(initialUser);

        // When
        var userWithDublicateUserName = UserBuilder.GenerateUserToCreate();
        userWithDublicateUserName.DisplayName = initialUser.DisplayName;
        var response = await client.SendCreateUserRequest(userWithDublicateUserName);

        // Then
        var wasPublished = _busTestFacade.WasPublished<UserCreatedEvent>("UserModule.UserCreated", x => x.UserName == initialUser.DisplayName && x.UserId == userWithDublicateUserName.UserId);


        Assert.False(wasPublished);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task GivenOneUserExists_WhenSomeoneSignsupWithTheSameUserName_ThenNoNewUserIsCreated()
    {
        //Given 
        var initialUser = UserBuilder.GenerateUserToCreate();
        await client.SendCreateUserRequest(initialUser);

        // When
        var requestWithDublicateName = UserBuilder.GenerateUserToCreate();
        requestWithDublicateName.DisplayName = initialUser.DisplayName;
        await client.PostAsJsonAsync("User", requestWithDublicateName);

        // Then
        var response = await client.GetAsync($"User/{requestWithDublicateName.UserId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


}
