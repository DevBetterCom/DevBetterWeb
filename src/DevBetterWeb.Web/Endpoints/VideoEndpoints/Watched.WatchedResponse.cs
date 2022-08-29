namespace DevBetterWeb.Web.Endpoints;

public class WatchedResponse
{
  public bool Success { get; set; }
  public string ResponseText { get; set; } = string.Empty;

  public WatchedResponse(bool success, string responseText)
  {
	  Success = success;
		ResponseText = responseText;
  }
}
