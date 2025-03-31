using EventMessages;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace UserModule.Tests.Tests;



public class CreateUserTests : IClassFixture<UserWebApplicationFactory>
{

    ITestHarness messageBrokerTestHarness;
    HttpClient client;
    ITestOutputHelper h;
    public CreateUserTests(UserWebApplicationFactory factory, ITestOutputHelper he)
    {
        h = he;
        client = factory.CreateClient();
        messageBrokerTestHarness = factory.Services.GetRequiredService<ITestHarness>();
    }
    [Fact]
    public async Task WhenUserSignsUpWithValidInformaiton_ThenSuccessIsReturned()
    {
        // When
        var requestBody = UserBuilder.CreateValidUserBody();
        var response = await client.PostAsJsonAsync("User",requestBody);

        // Then
        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode);        
    }       


    [Fact]
    public async Task WhenUserSignsUpWithValidInformaiton_ThenUserCreatedEventIsSent()
    {
        // When
        var requestBody = UserBuilder.CreateValidUserBody();
        await client.PostAsJsonAsync("User", requestBody);

        // Then
        FilterDelegate<IPublishedMessage<UserCreatedEvent>> filter = (msg) => msg.MessageType == typeof(UserCreatedEvent) &&
        (msg.MessageObject as UserCreatedEvent)?.UserId == requestBody.UserId &&
        (msg.MessageObject as UserCreatedEvent)?.UserName == requestBody.UserName;

        using var cts = new CancellationTokenSource(200);
        var eventRecieved =  await messageBrokerTestHarness.Published.Any<UserCreatedEvent>(filter, cts.Token);

        Assert.True(eventRecieved, "Expected event was not published in time");
    }

    [Fact]
    public async Task GivenOneUserExists_WhenSomeoneSignsupWithTheSameUserName_ThenBadRequestIsReturnedAndNoEventIsSent()
    {
        //Given 
        var initialuser = UserBuilder.CreateValidUserBody();
        await client.PostAsJsonAsync("User", initialuser);

        // When
        var userWithDublicateUserName = UserBuilder.CreateValidUserBody();
        userWithDublicateUserName.UserName = initialuser.UserName;
        var response = await client.PostAsJsonAsync("User", userWithDublicateUserName);

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
        var initialuser = UserBuilder.CreateValidUserBody();
        await client.PostAsJsonAsync("User", initialuser);

        // When
        var requestWithDublicateName = UserBuilder.CreateValidUserBody();
        requestWithDublicateName.UserName = initialuser.UserName;
        await client.PostAsJsonAsync("User", requestWithDublicateName);

        // Then
        var response = await client.GetAsync($"User/{requestWithDublicateName.UserId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
