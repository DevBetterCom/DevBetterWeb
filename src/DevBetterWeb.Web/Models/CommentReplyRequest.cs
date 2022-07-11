namespace DevBetterWeb.Web.Models;

public class CommentReplyRequest
{
  public string VideoId { get; set; } = string.Empty;
  public string CommentReplyToSubmit { get; set; } = string.Empty;
  public int? ParentCommentId { get; set; }
}
