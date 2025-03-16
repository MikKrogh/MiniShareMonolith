
using Microsoft.EntityFrameworkCore;
using PostsModule.Application;
using PostsModule.Domain;

namespace PostsModule.Infrastructure;

internal class PostsRepository : IPostsRepository
{
    private readonly ILogger<PostsRepository> logger;
    private readonly PostsContext context;

    public PostsRepository(ILogger<PostsRepository> logger, PostsContext context)
    {
        this.logger = logger;
        this.context = context;
    }

    public Task Delete(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<Post?> Get(string id)
    {
        var entity = await context.Posts.Include(post => post.Creator).Include(post => post.Images).FirstOrDefaultAsync<PostEntity>(x => x.Id == id);
        if (entity == null) return null;

        var post = Post.CreateNew(entity.Title, entity.CreatorId, entity.Faction);
        post.SetId(entity.Id);
        post.SetCreatorName(entity.Creator.UserName);
        post.SetCreationDate(entity.CreationDate);
        post.SetDescription(entity.Description);
        post.SetPrimaryColour(entity.PrimaryColour);
        post.SetSecondaryColour(entity.SecondaryColour);
        foreach (var image in entity.Images)
        {
            post.SetImages(image.Id);
        }
        return post;
    }

    public async Task<PaginationResult<Post>> GetAll(QueryModel query)
    {
        var postEntities =  context.Posts
            .Include(post => post.Creator)
            .Include(post => post.Images)
            .AsQueryable();

        if (postEntities?.Any() is not true) 
            return new PaginationResult<Post>();

        var totalCount = await postEntities.CountAsync(); 
        var queriedEntities = await postEntities.Take(totalCount).ToListAsync();

        List<Post> result = new();
        foreach (var postEntity in queriedEntities)
        {
            try
            {
                var post = Post.CreateNew(postEntity.Title, postEntity.CreatorId, postEntity.Faction, Guid.Parse(postEntity.Id));
                post.SetTitle(postEntity.Title);
                post.SetDescription(postEntity.Description);
                post.SetPrimaryColour(postEntity.PrimaryColour);
                post.SetSecondaryColour(postEntity.SecondaryColour);
                post.SetCreationDate(postEntity.CreationDate);
                post.SetCreatorName(postEntity.Creator.UserName);
                foreach (var image in postEntity.Images)
                {
                    post.SetImages(image.Id);
                }
                result.Add(post);
            }
            catch (Exception ex)
            {
                continue;
            }

        }
        return new PaginationResult<Post>()
        {
            TotalCount = totalCount,
            Items = result
        };

    }

    public async Task Save(Post post)
    {
        var creator = await context.Users.SingleOrDefaultAsync(x => x.Id == post.CreatorId);
        if (creator == null) throw new Exception("no user exists for the created post");
        var tableEntity = new PostEntity
        {
            Id = post.Id.ToString(),
            Title = post.Title,
            Description = post.Description,
            CreatorId = creator.Id,
            Faction = post.FactionName,
            PrimaryColour = post.PrimaryColour.ToString(),
            SecondaryColour = post.SecondaryColour.ToString(),
            CreationDate = post.CreationDate,
            Creator = creator
        };
        await context.Posts.AddAsync(tableEntity);
        await context.SaveChangesAsync();

    }
}
