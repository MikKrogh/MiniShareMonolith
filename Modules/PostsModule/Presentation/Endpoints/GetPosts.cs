using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PostsModule.Application;
using PostsModule.Application.GetPosts;

namespace PostsModule.Presentation.Endpoints;

public class GetPosts
{
    internal static async Task<IResult> Process([FromServices] IRequestClient<GetPostsCommand> client)
    {
        var command = new GetPostsCommand();
        var commandResult = await client.GetResponse<CommandResult<GetPostsCommandResult>>(command);
        var result = commandResult.Message;

        if (result.IsSuccess && result.ResultValue?.Posts is not null)        
            return Results.Ok(result.ResultValue.Posts);  
        return Results.StatusCode(result.ResultStatus);
    }
}
