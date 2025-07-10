using Microsoft.AspNetCore.Mvc;
using PostsModule.Application;
using PostsModule.Application.GetPosts;

namespace PostsModule.Presentation.Endpoints;

public class GetPosts
{
    internal static async Task<IResult> Process([FromServices] GetPostsCommandConsumer client, [FromQuery] int? take, string? orderBy, string? filter, string? search, int? skip)
    {
        var queryModel = new QueryModel()
        {
            Take = SetTake(take),
            Filter = filter,
            Search = search,
            Skip = skip ?? 0
        };

        if (!string.IsNullOrEmpty(orderBy))
        {
            queryModel = queryModel with
            {
                Descending = orderBy.EndsWith("desc"),
                OrderBy = orderBy.Split(' ')[0]
            };

        }
        var command = new GetPostsCommand()
        {
            QueryModel = queryModel
        };

        var commandResult = await client.Consume(command);

        if (commandResult.IsSuccess && commandResult.ResultValue?.Posts is not null)
        {
            var paginationResult = new PaginationResult<PostDto>()
            {
                TotalCount = commandResult.ResultValue.TotalCount,
                Items = commandResult.ResultValue.Posts
            };
            return Results.Ok(paginationResult);
        }
        return Results.StatusCode(commandResult.ResultStatus);
    }
    private static int SetTake(int? take)
    {
        if (take is null) return 100;
        if (take > 100) return 100;
        return take.Value;
    }
}
