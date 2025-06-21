using Microsoft.EntityFrameworkCore;

namespace EngagementModule;

public class EngagementDbContext : DbContext, IPostLikeService
{
    private readonly string connString;
    private  DbSet<PostLikeEntity> Likes { get; set; }
    public EngagementDbContext(DbContextOptions<EngagementDbContext> options,IConfiguration config, IWebHostEnvironment env):  base(options)
    {
        var connectionString = config["EngagementModuleConnString"];


        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException("Postgres connecion string must not be empty");
        else connString = connectionString;

        if (env.IsDevelopment() || env.IsEnvironment("Test"))
        {
            if (!Database.EnsureCreated()) {                 // If the database is not created, we apply migrations
                Database.Migrate();
                SaveChanges();
            }


        }
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    {
        optionsBuilder.UseNpgsql(connString, opt => opt.EnableRetryOnFailure(3));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostLikeEntity>().ToTable("PostLikes");        

        modelBuilder.Entity<PostLikeEntity>()
            .HasKey(l => new { l.UserId, l.PostId });
        modelBuilder.Entity<PostLikeEntity>()
            .Property(l => l.UserId)
            .IsRequired();
        modelBuilder.Entity<PostLikeEntity>()
            .Property(l => l.PostId)
            .IsRequired();
        base.OnModelCreating(modelBuilder);
    }
    public async Task Like(string postId, string userId)
    {
        await Likes.AddAsync(new PostLikeEntity { PostId = postId, UserId = userId });
        await SaveChangesAsync();
    }
    public async Task Unlike(string postId, string userId)
    {     
        await Likes.Where(l => l.PostId == postId && l.UserId == userId).ExecuteDeleteAsync();   
    }
    public async Task<int> GetLikesCount(string postId)
    {
        var count = await Likes.CountAsync(l => l.PostId == postId);
        return count;
    }
}

public interface IPostLikeService
{
    Task Like(string postId, string userId);  
    Task Unlike(string postId, string userId);
    Task<int> GetLikesCount(string postId);

}



public record PostLikeEntity
{
    public string? UserId { get; init; }
    public string? PostId { get; init; }
}