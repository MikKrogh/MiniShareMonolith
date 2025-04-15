
using Microsoft.EntityFrameworkCore;
using PostsModule.Domain;
using System.Linq.Expressions;

namespace PostsModule.Infrastructure;

internal class PostsRepository : IPostsRepository
{
    private readonly PostsContext context;

    public PostsRepository(PostsContext context)
    {
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
        post.SetPrimaryColor(entity.PrimaryColor);
        post.SetSecondaryColor(entity.SecondaryColor);
        foreach (var image in entity.Images)
        {
            post.SetImages(image.Id);
        }
        return post;
    }

    public async Task<PaginatedResult<Post>> GetAll(int take = 100, bool? descending = null, string? orderOnProperty = null, string? filter = null, string? search = null, int skip = 0)
    {
        IQueryable<PostEntity> postEntities = CreateQuery();
        postEntities = postEntities.AsNoTracking();
        postEntities = ApplyFilter(filter, postEntities);
        postEntities = ApplySearch(search, postEntities);
        var totalCount = await postEntities.CountAsync();
        postEntities = ApplyOrderBy(descending, orderOnProperty, postEntities);

        postEntities = postEntities.Skip(skip);

        var qstring = postEntities.ToQueryString();

        var queriedEntities = await postEntities.Take(take).ToListAsync();

        List<Post> result = MapToDomainEntities(queriedEntities);
        return new PaginatedResult<Post>()
        {
            TotalCount = totalCount,
            Items = result,
            PageSize = take,
        };
    }

    private static IQueryable<PostEntity> ApplySearch(string? search, IQueryable<PostEntity> postEntities)
    {
        if (!string.IsNullOrEmpty(search))
        {
            postEntities = postEntities.Where(x => x.Title.Contains(search) || x.Description.Contains(search));
        }

        return postEntities;
    }

    private static List<Post> MapToDomainEntities(List<PostEntity> queriedEntities)
    {
        List<Post> result = new();
        foreach (var postEntity in queriedEntities)
        {
            try
            {
                var post = Post.CreateNew(postEntity.Title, postEntity.CreatorId, postEntity.Faction, Guid.Parse(postEntity.Id));
                post.SetTitle(postEntity.Title);
                post.SetDescription(postEntity.Description);
                post.SetPrimaryColor(postEntity.PrimaryColor);
                post.SetSecondaryColor(postEntity.SecondaryColor);
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

        return result;
    }

    private IQueryable<PostEntity> CreateQuery()
    {
        return context.Posts
            .Include(post => post.Creator)
            .Include(post => post.Images)
            .AsQueryable();
    }

    private static IQueryable<PostEntity> ApplyOrderBy(bool? descending, string? orderOnProperty, IQueryable<PostEntity> postEntities)
    {
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
            catch (Exception e) { }
        }

        return postEntities;
    }

    private static IQueryable<PostEntity> ApplyFilter(string? filter, IQueryable<PostEntity> postEntities)
    {
        if (!string.IsNullOrEmpty(filter))
        {
            var filters = OdataFilterReader.Read(filter);
            if (filters?.Any() is true)
            {
                var parameter = Expression.Parameter(typeof(PostEntity), "x");
                Expression combinedFilterExpression = null;
                foreach (var criteria in filters)
                {
                    var property = typeof(PostEntity).GetProperty(criteria.PropertyName);
                    if (property == null) continue;

                    var propertyAccess = Expression.Property(parameter, property);
                    var constantValue = Expression.Constant(Convert.ChangeType(criteria.Value, property.PropertyType));

                    Expression? expression = criteria.Operator switch
                    {
                        "eq" => Expression.Equal(propertyAccess, constantValue),
                        "gt" => Expression.GreaterThan(propertyAccess, constantValue),
                        "lt" => Expression.LessThan(propertyAccess, constantValue),
                        _ => null
                    };
                    if (expression != null)
                    {
                        combinedFilterExpression = combinedFilterExpression == null
                            ? expression
                            : Expression.AndAlso(combinedFilterExpression, expression);
                    }
                }
                if (combinedFilterExpression != null)
                {
                    var lambda = Expression.Lambda<Func<PostEntity, bool>>(combinedFilterExpression, parameter);
                    postEntities = postEntities.Where(lambda);
                }
            }   
        }
        return postEntities;
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
            PrimaryColor = post.PrimaryColor.ToString().ToLower(),
            SecondaryColor = post.SecondaryColor.ToString().ToLower(),
            CreationDate = post.CreationDate,
            Creator = creator
        };
        await context.Posts.AddAsync(tableEntity);
        await context.SaveChangesAsync();

    }
}

public static class OdataFilterReader
{
    public static List<FilterCriteria>? Read(string odataString)
    {
        if (odataString[0] == '$')
            odataString = odataString.Remove(0, 1);
        if (odataString.StartsWith("filter="))
            odataString = odataString.Remove(0, 7);
        odataString = odataString.Replace(" And ", " and ", StringComparison.InvariantCultureIgnoreCase);
        odataString = odataString.Replace(" And ", " and ");

        var filterStrings = odataString.Split("and");

        var filtermodels = new List<FilterCriteria>();
        foreach (var filter in filterStrings)
        {
            var split = filter.Trim().Split(' ');
            var t = new FilterCriteria
            {
                PropertyName = split[0],
                Operator = split[1],
                Value = split[2].Replace("'", "")
            };
            filtermodels.Add(t);
        }
        return filtermodels;
    }
    public class FilterCriteria
    {
        public string PropertyName { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
}