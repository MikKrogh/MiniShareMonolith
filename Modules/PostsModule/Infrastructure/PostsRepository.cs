
using Microsoft.EntityFrameworkCore;
using PostsModule.Application;
using PostsModule.Domain;
using System.Linq;
using System.Linq.Expressions;
using static MassTransit.ValidationResultExtensions;

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

    public async Task<PaginatedResult<Post>> GetAll(int take = 100, bool? descending = null, string? orderOnProperty = null)
    {
        var postEntities =  context.Posts
            .Include(post => post.Creator)
            .Include(post => post.Images)
            .AsQueryable();
        var totalCount = await postEntities.CountAsync();

        if (!string.IsNullOrEmpty(orderOnProperty))
        {
            try
            {
                var propertyToOrderOn = typeof(PostEntity).GetProperty(orderOnProperty);
                var parameter = Expression.Parameter(typeof(PostEntity), "x");
                var propertyAccess = Expression.Property(parameter, propertyToOrderOn);
                var orderByExpression = Expression.Lambda(propertyAccess, parameter);

                var methodName = (descending.HasValue && descending.Value)
                 ? "OrderByDescending"
                 : "OrderBy";

                var resultExpression = Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { typeof(PostEntity), propertyToOrderOn.PropertyType },
                    postEntities.Expression,
                    Expression.Quote(orderByExpression)
                );
                postEntities = postEntities.Provider.CreateQuery<PostEntity>(resultExpression);

            }
            catch (Exception e)
            {

            }

        }
        try
        {
            var queriedEntities = await postEntities.Take(take).ToListAsync();

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
        return new PaginatedResult<Post>()
        {
            TotalCount = totalCount,
            Items = result,
            PageSize = take,
        };
        }
        catch (Exception e)
        {
            return new PaginatedResult<Post>()
            {
                TotalCount = 0,
                Items = null,
                PageSize = take,
            };
        }

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
