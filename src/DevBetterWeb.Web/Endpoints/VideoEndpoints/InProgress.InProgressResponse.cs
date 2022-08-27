namespace DevBetterWeb.Web.Endpoints;

public class InProgressResponse
{
  public bool Success { get; set; }
  public string ResponseText { get; set; } = string.Empty;

  public InProgressResponse(bool success, string responseText)
  {
	  Success = success;
		ResponseText = responseText;
  }
}
