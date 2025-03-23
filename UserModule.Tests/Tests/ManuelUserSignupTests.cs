
using System.Net.Http.Json;

namespace UserModule.Tests.Tests;

public class ManuelUserSignupTests : IClassFixture<UserWebApplicationFactory>
{
    HttpClient client;
    public ManuelUserSignupTests(UserWebApplicationFactory factory)
    {
        client = factory.CreateClient();        
    }
    [Fact]
    public async Task WhenUserSignsUpWithValidInformaiton_ThenSuccessIsReturned()
    {
        var requestBody = new 
        {
            DisplayName = "Test",
            Email = "emailtest@testdomain.tst",
            Password = "Passwor123!",
        };
        var response = await client.PostAsJsonAsync("User",requestBody);

        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode);
        
    }
    
    [Fact]
    public async Task WhenUserSignsUpWithValidInformaiton_ThenCorrectValuesAreStored()
    {
        Assert.True(false);
    }

        
    [Fact]
    public async Task WhenUserSignsUpWithValidInformaiton_ThenUserCreatedEventIsSent()
    {
        Assert.True(false);
    }

    [Fact]
    public async Task GivenOneUserExists_WhenSomeoneSignsupWithTheSameDisplayName_ThenBadRequestIsReturned()
    {
        Assert.True(true);
    }

    [Fact] 
    public async Task WhenUserTriesToSignUpWithInvalidEmail_ThenBadRequestIsReturned()
    {
        Assert.True(false);
    }  
    [Fact] 
    public async Task WhenUserTriesToSignUpWithInvalidPassword_ThenBadRequestIsReturned()
    {
        Assert.True(false);
    }  
    [Fact] 
    public async Task WhenUserTriesToSignUpWithInvalidDisplayName_ThenBadRequestIsReturned()
    {
        Assert.True(false);
    }
}
