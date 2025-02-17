

using MassTransit.Testing;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Sdk;
using PostsModule.Application.UserEvents;
using static System.Formats.Asn1.AsnWriter;

namespace PostsModule.Tests.UserEventHandlerTests;

public class UserCreatedEventHandlerTests : IClassFixture<PostsWebApplicationFactory>
{
    private readonly PostsWebApplicationFactory _factory;
    private readonly HttpClient _client;
    public UserCreatedEventHandlerTests(PostsWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    //WhenNewUserIsCreated_ThenUserIsSaved
    [Fact]
    public async Task WhenNewUserIsCreated_ThenUserIsSaved()
    {
        var testHarness = _factory.Services.GetTestHarness();
        var bus = _factory.Services.GetService<IBus>(); 
        var harness = _factory.Services.GetService<ITestHarness>();


        // Publish a test message
        await bus.Publish(new UserCreatedEvent { UserId = Guid.NewGuid(), UserName = "mike hawn" });

        //// Ensure the consumer received the message
        Assert.True(await harness.Consumed.Any<UserCreatedEvent>());
        Assert.True(await harness.Published.Any<Fault<UserCreatedEvent>>(), "No fault message was published");

    }
    //GivenUserExistsWithOriginalName_WhenUserCreatedEventIsRecievedForExistingUserWithNewName_ThenOriginalNameIsSaved

}
