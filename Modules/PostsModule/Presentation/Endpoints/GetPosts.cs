using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application;
using PostsModule.Application.GetPosts;

namespace PostsModule.Presentation.Endpoints;

public class GetPosts
{
    internal static async Task<IResult> Process([FromServices] IRequestClient<GetPostsCommand> client, [FromQuery]int? take)
    {
        var command = new GetPostsCommand();
        var queryModel = new QueryModel()
        {
            Take = take ?? 100
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
}
