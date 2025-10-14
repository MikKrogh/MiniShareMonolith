using Microsoft.EntityFrameworkCore;
namespace UserModule;

public interface IUserRepository
{
    Task CreateUser(User user);
    Task<User?> GetUser(string userId);
    Task<IEnumerable<User>> GetUsers(IReadOnlyCollection<string> userIds);
}

public class UserDbContext : DbContext, IUserRepository
{
    private readonly string connString;
    public DbSet<User> Users { get; set; }

    public UserDbContext(DbContextOptions<UserDbContext> options, IConfiguration config, IWebHostEnvironment env) : base(options)
    {
        connString = config["UserModuleConnString"];
        if (string.IsNullOrEmpty(connString))
            throw new Exception("Cannot initialize UserDbContext without a connectionstring");
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connString, options =>
        {
            options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null);
            options.CommandTimeout(10);
        });

        base.OnConfiguring(optionsBuilder);

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users", "UserModule");

        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();
    }

    public async Task CreateUser(User user)
    {
        Users.Add(user);
        await SaveChangesAsync();        

    }

    public async Task<User?> GetUser(string userId)
    {
        return await Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<IEnumerable<User>> GetUsers(IReadOnlyCollection<string> userIds)
    {
        return await Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
    }
}
