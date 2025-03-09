using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("PostsModule.Tests")]
namespace PostsModule.Infrastructure;

internal class PostsContext : DbContext
{
    private readonly IConfiguration config;

    public DbSet<PostEntity> Posts { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<ImageEntity> Images { get; set; }
    public string DbPath { get; }

    public PostsContext(DbContextOptions<PostsContext> options, IConfiguration config) : base(options)
    {
        this.config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(config.GetConnectionString("sql"));
        base.OnConfiguring(optionsBuilder);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>().ToTable("Users", "StoreSchema");
        modelBuilder.Entity<PostEntity>().ToTable("Posts", "StoreSchema");
        modelBuilder.Entity<ImageEntity>().ToTable("Image", "StoreSchema");

        modelBuilder.Entity<PostEntity>()
            .HasOne(p => p.Creator)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.CreatorId);

        modelBuilder.Entity<ImageEntity>()
            .HasOne(i => i.Post)
            .WithMany(p => p.Images)
            .HasForeignKey(i => i.PostId);
    }
}
