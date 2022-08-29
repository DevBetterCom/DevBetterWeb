namespace DevBetterWeb.Web.Endpoints;

public class UnWatchedResponse
{
  public bool Success { get; set; }
  public string ResponseText { get; set; } = string.Empty;

  public UnWatchedResponse(bool success, string responseText)
  {
	  Success = success;
		ResponseText = responseText;
  }
}
