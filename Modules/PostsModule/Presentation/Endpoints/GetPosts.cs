using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application;
using PostsModule.Application.GetPosts;

namespace PostsModule.Presentation.Endpoints;

public class GetPosts
{
    //orderby should follow oData specicitaion. Example "orderBy=propertyName Desc" or "orderBy=propertyName"
    internal static async Task<IResult> Process([FromServices] IRequestClient<GetPostsCommand> client, [FromQuery]int? take, string? orderBy, string? filter)
    {
        var queryModel = new QueryModel()
        {
            Take = take ?? 100,  
            Filter = filter
        };

        if (!string.IsNullOrEmpty(orderBy))
        {
            queryModel.Descending = orderBy.EndsWith("desc");
            queryModel.OrderBy = orderBy.Split(' ')[0];
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
}
