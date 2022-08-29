namespace DevBetterWeb.Web.Endpoints;

public class MarkFavoriteVideoResponse
{
  public bool Success { get; set; }
  public string ResponseText { get; set; } = string.Empty;

  public MarkFavoriteVideoResponse(bool success, string responseText)
  {
	  Success = success;
		ResponseText = responseText;
  }
}
