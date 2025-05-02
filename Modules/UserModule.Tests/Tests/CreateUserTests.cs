using EventMessages;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;
using static UserModule.Tests.UserBuilder;

namespace UserModule.Tests.Tests;

public class CreateUserTests : IClassFixture<UserWebApplicationFactory>
{
    ITestHarness messageBrokerTestHarness;
    HttpClient client;
    public CreateUserTests(UserWebApplicationFactory factory, ITestOutputHelper he)
    {
        client = factory.CreateClient();
        messageBrokerTestHarness = factory.Services.GetRequiredService<ITestHarness>();
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
        // When
        var userToCreate = UserBuilder.GenerateUserToCreate();
        await client.SendCreateUserRequest(userToCreate);

        // Then
        FilterDelegate<IPublishedMessage<UserCreatedEvent>> filter = (msg) => msg.MessageType == typeof(UserCreatedEvent) &&
        (msg.MessageObject as UserCreatedEvent)?.UserId == userToCreate.UserId &&
        (msg.MessageObject as UserCreatedEvent)?.UserName == userToCreate.DisplayName;

        using var cts = new CancellationTokenSource(200);
        var eventRecieved = await messageBrokerTestHarness.Published.Any<UserCreatedEvent>(filter, cts.Token);

        Assert.True(eventRecieved, "Expected event was not published in time");
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
        FilterDelegate<IPublishedMessage<UserCreatedEvent>> filter = (msg) => msg.MessageType == typeof(UserCreatedEvent) &&
        (msg.MessageObject as UserCreatedEvent).UserId == invalidId;

        using var cts = new CancellationTokenSource(200);
        var eventsRecieved = messageBrokerTestHarness.Published.Select(filter, cts.Token);

        Assert.Empty(eventsRecieved);
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
        FilterDelegate<IPublishedMessage<UserCreatedEvent>> filter = (msg) => msg.MessageType == typeof(UserCreatedEvent) &&
        (msg.MessageObject as UserCreatedEvent).UserId == userWithDublicateUserName.UserId;

        using var cts = new CancellationTokenSource(200);
        var eventsRecieved = messageBrokerTestHarness.Published.Select(filter, cts.Token);

        Assert.Empty(eventsRecieved);
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
