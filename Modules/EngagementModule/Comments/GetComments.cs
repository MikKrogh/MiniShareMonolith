using Microsoft.AspNetCore.Mvc;

namespace EngagementModule.Comments;

internal static class GetComments
{
    public static async Task<IResult> Process([FromServices] ICommentService service,[FromRoute]string postId)
    {
        var comments = await service.GetComments(postId);
        var commentDtos = comments.Select(c => new CommentDto
        {
            CommentId = c.CommentId,
            ParentCommentId = c.ParentCommentId,
            UserId = c.UserId,
            Content = c.Content,
            CreatedAt = c.CreatedAt
        }).ToList();
        return Results.Ok(commentDtos);
    }

}

public  record CommentDto
{
    public string CommentId { get; init; } = string.Empty;
    public string? ParentCommentId { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
};