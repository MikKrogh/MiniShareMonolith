using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application;
using PostsModule.Application.GetPosts;

namespace PostsModule.Presentation.Endpoints;

public class GetPosts
{
    internal static async Task<IResult> Process([FromServices] IRequestClient<GetPostsCommand> client, [FromQuery] int? take, string? orderBy, string? filter, string? search, int? skip)
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

        var commandResult = await client.GetResponse<CommandResult<GetPostsCommandResult>>(command);
        var result = commandResult.Message;

        if (result.IsSuccess && result.ResultValue?.Posts is not null)
        {
            var paginationResult = new PaginationResult<PostDto>()
            {
                TotalCount = result.ResultValue.TotalCount,
                Items = result.ResultValue.Posts
            };
            return Results.Ok(paginationResult);
        }
        return Results.StatusCode(result.ResultStatus);
    }
    private static int SetTake(int? take)
    {
        if (take is null) return 100;
        if (take > 100) return 100;
        return take.Value;
    }
}
