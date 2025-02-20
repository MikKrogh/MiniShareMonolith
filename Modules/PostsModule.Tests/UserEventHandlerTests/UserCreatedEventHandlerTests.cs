

using MassTransit.Testing;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Sdk;
using PostsModule.Application.UserEvents;
using static System.Formats.Asn1.AsnWriter;

namespace PostsModule.Tests.UserEventHandlerTests;

public class UserCreatedEventHandlerTests : IClassFixture<PostsWebApplicationFactory>
{
    private readonly MessageBrokerTestFacade messageBroker;
    private readonly PostsWebApplicationFactory _factory;
    private readonly HttpClient _client;
    public UserCreatedEventHandlerTests(PostsWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        messageBroker = factory.MessageBrokerTestFacade;
    }

    //WhenNewUserIsCreated_ThenUserIsSaved
    [Fact]
    public async Task WhenNewUserIsCreated_ThenUserIsSaved()
    {

        var facade = _factory.Services.GetRequiredService<MessageBrokerTestFacade>();

        // Publish a test message
        await facade.Publish(new UserCreatedEvent { UserId = Guid.NewGuid(), UserName = "mike hawk" });
        await facade.Publish(new UserCreatedEvent { UserId = Guid.NewGuid(), UserName = "Jill hawn" });

        
        var biggerPredicate  = new Predicate<UserCreatedEvent>(x =>
            !string.IsNullOrEmpty(x.UserId.ToString()) &&
            x.UserName == "mike hawk"
        );



        await facade.AssertExactlyOneMessageMatch(biggerPredicate);



    }
    //GivenUserExistsWithOriginalName_WhenUserCreatedEventIsRecievedForExistingUserWithNewName_ThenOriginalNameIsSaved

}
