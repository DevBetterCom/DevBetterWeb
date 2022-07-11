namespace DevBetterWeb.Web.Models;

public class CommentReplyRequest
{
  public string VideoId { get; set; }
  public string CommentReplyToSubmit { get; set; }
  public int? ParentCommentId { get; set; }
}
