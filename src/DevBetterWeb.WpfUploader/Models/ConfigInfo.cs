namespace DevBetterWeb.WpfUploader.Models;

public class ConfigInfo
{
  public string Token { get; private set; }
  public string ApiLink { get; private set; }
  public string ApiKey { get; private set; }

  public ConfigInfo(string token, string apiLink, string apiKey)
  {
    Token = token;
    ApiLink = apiLink;
    ApiKey = apiKey;
  }

  public void Update(string token, string apiLink, string apiKey)
  {
    Token = token;
    ApiLink = apiLink;
    ApiKey = apiKey;
  }
}
