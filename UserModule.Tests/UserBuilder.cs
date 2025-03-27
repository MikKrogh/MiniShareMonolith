

namespace UserModule.Tests;


internal class UserBuilder
{

    public static UserToCreate CreateValidUserBody()
    {
        string id = Guid.NewGuid().ToString();
        string uniqueSuffix = id.Substring(0, 8);

        var requestBody = new UserToCreate
        {
            UserId = id,
            UserName = $"UserName{uniqueSuffix}",
        };
        return requestBody;
    }

    internal class UserToCreate
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}