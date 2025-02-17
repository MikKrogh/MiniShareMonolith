
using Microsoft.EntityFrameworkCore;
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
         var entity = await context.FindAsync<PostEntity>(id);       
        if (entity == null) return null;

        var post =  Post.CreateNew(entity.Title, entity.CreatorId, entity.Faction);
        post.SetId(entity.Id);
        post.SetCreatorName(entity.Creator.UserName);
        post.SetCreationDate(entity.CreationDate);
        post.SetDescription(entity.Description);
        post.SetPrimaryColour(entity.PrimaryColour);
        post.SetSecondaryColour(entity.SecondaryColour);

        return post;


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
