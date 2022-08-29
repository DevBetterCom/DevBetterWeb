namespace DevBetterWeb.Web.Endpoints;

public class SubmitCommentReplyResponse
{
  public bool Success { get; set; }
  public string ResponseText { get; set; } = string.Empty;

  public SubmitCommentReplyResponse(bool success, string responseText)
  {
	  Success = success;
		ResponseText = responseText;
  }
}
