using Microsoft.AspNetCore.Mvc;

namespace EngagementModule.Comments;

internal class AddComment
{
    public static async Task<IResult> Process([FromServices] ICommentService service,[FromRoute]string postId, [FromQuery]string userId,[FromBody]AddCommentDto dto)
    {        
        var entity = new CommentEntity
        {
            PostId = postId,
            UserId = userId,
            Content = dto.Content,
            ParentCommentId = string.IsNullOrEmpty(dto.ParentCommentId) ? null : dto.ParentCommentId,
        };

        if (IsValid(entity))
        {
            await service.AddComment(entity);
            return Results.Ok(entity.CommentId);
        }
        return Results.BadRequest();
    }

    private static bool IsValid(CommentEntity entity)
    {
        if (string.IsNullOrEmpty(entity.UserId) || string.IsNullOrEmpty(entity.Content))
        {
            return false;
        }
        return true;
    }
}

internal record AddCommentDto
{
    public string Content { get; set; } = string.Empty;
    public string? ParentCommentId { get; set; } = null;

};