using Azure;
using Azure.Data.Tables;
using Azure.Identity;
using MassTransit.Testing;
namespace UserModule;


public interface IUserRepository
{
    Task CreateUser(User user);
    Task<User> GetUser(string userId);
}

internal class UserRepository : IUserRepository
{
    public const string partitionKey = "global";

    private bool tableExists = false;
    private readonly TableClient tableClient;
    private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1); // Controls access

    public UserRepository(IConfiguration config)
    {
        var connString = config["TableStorageAccount"] ?? throw new Exception("Cannot initialize UserRepository without a connectionstring");

        if (connString == "UseDevelopmentStorage=true;")
            tableClient = new(connString, "User");

        else
            tableClient = new TableClient(new Uri(connString), "User", new DefaultAzureCredential());



    }

    public async Task<User?> GetUser(string userId)
    {
        await CreateTable();
        try
        {
            var userEntity = await tableClient.GetEntityAsync<UserEntity>(partitionKey, userId);
            if (userEntity == null) return null;

            var user = new User
            {
                Id = userEntity.Value.Id,
                UserName = userEntity.Value.UserName,
                CreationDate = userEntity.Value.CreationDate,
            };
            return user;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task CreateUser(User user)
    {
        await CreateTable();

        if (await NameIsOccupied(user.UserName))
            throw new Exception("Cannot add user becouse username already exists");
        var userEntity = new UserEntity()
        {
            RowKey = user.Id,
            PartitionKey = partitionKey,
            CreationDate = DateTime.UtcNow,
            UserName = user.UserName,
        };
        await tableClient.AddEntityAsync(userEntity);
    }

    private async Task CreateTable()
    {
        if (tableExists) return;
        await semaphore.WaitAsync();
        try
        {
            try
            {
                var result = await tableClient.CreateIfNotExistsAsync();
                tableExists = true;
            }
            catch (Exception e)
            {

            }


        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task<bool> NameIsOccupied(string displayName)
    {
        var getByUserName = tableClient.QueryAsync<UserEntity>(x => x.UserName == displayName);
        var NameIsOccupied = await getByUserName.Any();
        return NameIsOccupied;
    }

    private class UserEntity : ITableEntity
    {
        public string Id => RowKey;
        public string UserName { get; init; }

        public DateTime CreationDate { get; init; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

    }
}

