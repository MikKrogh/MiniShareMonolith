using Azure;
using Azure.Data.Tables;
using Azure.Identity;
namespace UserModule;


public interface IUserRepository
{
    Task CreateUser(User user);
    Task<User> GetUser(string userId);
    Task<IEnumerable<User>> GetUsers(IReadOnlyCollection<string> userIds);
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
    public async Task<IEnumerable<User>> GetUsers(IReadOnlyCollection<string> userIds)
    {
        await CreateTable();
        //string odataStringStart = "PartitionKey eq '" + partitionKey + "' " +
        //    "AND (Rowkey eq "; 
        //string odataStringMiddle = string.Join(" or RowKey eq ", userIds.Select(x => $"'{x}'"));
        //string odataStringEnd = ")";
        //string odataString = odataStringStart + odataStringMiddle + odataStringEnd;


        var filters = userIds.Select(rk => TableClient.CreateQueryFilter<UserEntity>(e =>  e.RowKey == rk))
        .ToList();

        // Combine all filters using OR
        string combined = "PartitionKey eq '" + partitionKey + "' " +
              "AND (";
        foreach (var item in filters)
        {
            combined += item + " or ";
        }
        string cleaned = combined.TrimEnd(' ', 'o', 'r', ' ');
        cleaned += ")";
        try
        {
            var userQuery = tableClient.QueryAsync<UserEntity>(cleaned);

            var users = new List<User>();
            await foreach (var userEntity in userQuery)
            {
                var user = new User
                {
                    Id = userEntity.Id,
                    UserName = userEntity.UserName,
                    CreationDate = userEntity.CreationDate,
                };
                users.Add(user);
            }
            return users;
        }
        catch (Exception e)
        {

            throw;
        }






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
            var result = await tableClient.CreateIfNotExistsAsync();
            tableExists = true;
        }
        catch (Exception e)
        {
            throw new Exception("Cannot create table in tableStorage", e);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task<bool> NameIsOccupied(string displayName)
    {
        var getByUserName = tableClient.QueryAsync<UserEntity>(x => x.UserName == displayName);
        await foreach (var item in getByUserName)
        {
            return true;
        }
        return false ;
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

