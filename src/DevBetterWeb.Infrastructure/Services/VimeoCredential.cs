namespace DevBetterWeb.Infrastructure.Services
{
  public class VimeoCredential
  {
    public string? ClientId { get; }
    public string? ClientSecret { get; }
    public string? RedirectionUrl { get; }
    public string? StateInformation { get; }
    public string? AccessToken { get; }

    public VimeoCredential(string clientId, string clientSecret, string redirectionUrl, string? stateInformation=null)
    {
      ClientId = clientId;
      ClientSecret = clientSecret;
      RedirectionUrl = redirectionUrl;
      StateInformation = stateInformation;
    }

    public VimeoCredential(string accessToken)
    {
      AccessToken = accessToken;
    }
  }
}
