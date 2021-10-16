namespace DevBetterWeb.UploaderApp
{
  public class ConfigInfo
  {
    public string Token { get; }
    public string ApiLink { get; }
    public string ApiKey { get; }

    public ConfigInfo(string token, string apiLink, string apiKey)
    {
      Token = token;
      ApiLink = apiLink;
      ApiKey = apiKey;
    }
  }
}
