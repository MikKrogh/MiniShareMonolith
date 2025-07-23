using EngagementModule.Comments;
using EngagementModule.Notification.PostCreated;
using Microsoft.EntityFrameworkCore;

namespace EngagementModule;

internal class EngagementDbContext : DbContext, IPostLikeService, ICommentService, ChainActivityService
{
    private readonly string connString;
    private  DbSet<PostLikeEntity> Likes { get; set; }
    private  DbSet<CommentEntity> Comments { get; set; }
    private  DbSet<ActivityChain> ActivityChains { get; set; }
    private  DbSet<ChainLink> ChainListeners { get; set; }
    private  DbSet<UserLastSync> UserSyncs { get; set; }
    
    public EngagementDbContext(DbContextOptions<EngagementDbContext> options,IConfiguration config, IWebHostEnvironment env):  base(options)
    {
        var connectionString = config["EngagementModuleConnString"];


        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException("Postgres connecion string must not be empty");
        else connString = connectionString;
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    {
        optionsBuilder.UseNpgsql(connString, opt => opt.EnableRetryOnFailure(3));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<PostLikeEntity>().ToTable("PostLikes");        
        modelBuilder.Entity<CommentEntity>().ToTable("PostComments");        
        modelBuilder.Entity<ActivityChain>().ToTable("ActivityChains");        
        modelBuilder.Entity<UserLastSync>().ToTable("UserSync");        
        modelBuilder.Entity<ChainLink>().ToTable("ChainListeners");

        modelBuilder.Entity<PostLikeEntity>()
            .HasKey(l => new { l.UserId, l.PostId });
        modelBuilder.Entity<PostLikeEntity>()
            .Property(l => l.UserId)
            .IsRequired();
        modelBuilder.Entity<PostLikeEntity>()
            .Property(l => l.PostId)
            .IsRequired();

        modelBuilder.Entity<CommentEntity>()
            .HasKey(c => c.CommentId);
        modelBuilder.Entity<CommentEntity>().Property(c => c.PostId)
            .IsRequired();
        modelBuilder.Entity<CommentEntity>().Property(c => c.UserId)
            .IsRequired();

        modelBuilder.Entity<UserLastSync>().HasKey(x => x.UserId);
        modelBuilder.Entity<ChainLink>().HasKey(x => new { x.UserId, x.AcitivtyChainId });
        modelBuilder.Entity<ChainLink>()
            .HasOne(x => x.Chain)
            .WithMany(c => c.Chains)
            .HasForeignKey(x => x.AcitivtyChainId);

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
    public async Task<bool> HasLiked(string postId, string userId)
    {
        var hasLiked = await Likes.AnyAsync(l => l.PostId == postId && l.UserId == userId);
        return hasLiked;
    }

    public async Task AddComment(CommentEntity comment)
    {
        await Comments.AddAsync(comment);
        if (string.IsNullOrEmpty(comment.ParentCommentId))
        {
            var postCreatorChain = await ActivityChains                
                .FirstOrDefaultAsync(c => c.Id == comment.PostId);
            if (postCreatorChain is not null)
                postCreatorChain.DateChanged = DateTime.UtcNow;

            var rootCommentChain = new ActivityChain
            {
                Id = comment.CommentId,
                PostId = comment.PostId,
                DateChanged = DateTime.UtcNow,
                Chains = new List<ChainLink>()

            };
            var chainlink = new ChainLink
            {
                UserId = comment.UserId,
                AcitivtyChainId = rootCommentChain.Id,
                Chain = rootCommentChain
            };
            rootCommentChain.Chains.Add(chainlink);
            ActivityChains.Add(rootCommentChain);
        }
        else
        {
            var chain = await ActivityChains
                .Include(c => c.Chains)
                .FirstOrDefaultAsync(c => c.Id == comment.ParentCommentId);
            if (chain is not null)
            {
                chain.DateChanged = DateTime.UtcNow;
                var existingLink = chain.Chains.FirstOrDefault(c => c.UserId == comment.UserId && c.AcitivtyChainId == chain.Id);
                if (existingLink is null)
                {
                    var newLink = new ChainLink
                    {
                        UserId = comment.UserId,
                        AcitivtyChainId = chain.Id,
                        Chain = chain
                    };
                    chain.Chains.Add(newLink);
                }
            }
        }
        await SaveChangesAsync();
    }

    public Task<List<CommentEntity>> GetComments(string postId)
    {
        return Comments.Where(c => c.PostId == postId).ToListAsync();
    }

    public async Task CreateChain(ActivityChain entity)
    {
        ActivityChains.Add(entity);
        await SaveChangesAsync();
    }

    public Task AddChainLink(ChainLink entity)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<string>> GetPostsWithNewComments(string userId)
    {
        var lastsync = await UserSyncs.FindAsync(userId);
        if (lastsync is null) return Enumerable.Empty<string>();

        var postIds = await ChainListeners
            .Where(cl => cl.UserId == userId && cl.Chain.DateChanged > lastsync.LastSyncTime)
            .Select(cl => cl.Chain.PostId)
            .Distinct()
            .ToListAsync();

        return postIds == null 
            ? Enumerable.Empty<string>() 
            : postIds;
        

    }

    public async Task UpdateLastSync(string userId)
    {
        var lastsync = await UserSyncs.FindAsync(userId);
        if (lastsync is null)
        {
            lastsync = new UserLastSync { UserId = userId, LastSyncTime = DateTime.UtcNow };
            await UserSyncs.AddAsync(lastsync);
        }
        else
        {
            lastsync.LastSyncTime = DateTime.UtcNow;
            UserSyncs.Update(lastsync);
        }
        await SaveChangesAsync();
    }
}

public interface IPostLikeService
{
    Task Like(string postId, string userId);
    Task<bool> HasLiked(string postId, string userId);     
    Task Unlike(string postId, string userId);
    Task<int> GetLikesCount(string postId);

}
internal interface ChainActivityService
{
    Task CreateChain(ActivityChain entity);
    Task AddChainLink(ChainLink entity);
    Task<IEnumerable<string>> GetPostsWithNewComments(string userId);
    Task UpdateLastSync(string userId);
}

internal interface ICommentService
{
    Task AddComment(CommentEntity comment);
    Task<List<CommentEntity>> GetComments(string postId);

}



public record PostLikeEntity
{
    public string? UserId { get; init; }
    public string? PostId { get; init; }
}