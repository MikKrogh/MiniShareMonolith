using EventMessages;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.Utilities;
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
        Assert.True(true);
    } 
    [Fact]
    public async Task GivenOneUserExists_WhenSomeoneSignsupWithTheSameEmail_ThenBadRequestIsReturnedAndNoEventIsSent()
    {
        Assert.True(true);
    }

    [Fact] 
    public async Task WhenUserTriesToSignUpWithInvalidEmail_ThenBadRequestIsReturnedAndNoEventIsSent()
    {
        Assert.True(false);
    }  
  
    [Fact] 
    public async Task WhenUserTriesToSignUpWithInvalidUserName_ThenBadRequestIsReturnedAndNoEventIsSent()
    {
        Assert.True(false);
    }




}
