using System.Collections.Generic;
using System.Threading.Tasks;
using VimeoDotNet;
using VimeoDotNet.Authorization;
using VimeoDotNet.Models;

namespace DevBetterWeb.Infrastructure.Services
{
  public class VimeoService
  {
    private static readonly string PUBLIC_SCOPE = "public";
    private static readonly string PRIVATE_SCOPE = "private";
    private static readonly string PURCHASED_SCOPE = "purchased";
    private static readonly string CREATE_SCOPE = "create";
    private static readonly string EDIT_SCOPE = "edit";
    private static readonly string DELETE_SCOPE = "delete";
    private static readonly string INTERACT_SCOPE = "interact";
    private static readonly string UPLOAD_SCOPE = "upload";
    private static readonly string PROMO_CODE_SCOPE = "promo_codes";
    private static readonly string VIDEO_FILES_SCOPE = "video_files";

    private readonly VimeoClient _vimeoClient;
    private readonly AuthorizationClient _authorizationClient;
    private readonly VimeoCredential _vimeoCredential;
    public VimeoService(VimeoCredential vimeoCredential)
    {
      _vimeoCredential = vimeoCredential;
      _vimeoClient = new VimeoClient(_vimeoCredential.ClientId, _vimeoCredential.ClientSecret);
      _authorizationClient = new AuthorizationClient(_vimeoCredential.ClientId, _vimeoCredential.ClientSecret);
    }

    public async Task<string> GetFullAccessTokenAsync()
    {
      var fullAccessUrl = GetFullAccessUrl();
      AccessTokenResponse accessToken =  await _vimeoClient.GetAccessTokenAsync("thfthfgh", _vimeoCredential?.RedirectionUrl);

      return accessToken.AccessToken;
    }

    public async Task<string> GetReadAccessTokenAsync()
    {
      var readAccessUrl = GetReadAccessUrl();
      AccessTokenResponse accessToken = await _vimeoClient.GetAccessTokenAsync(readAccessUrl, _vimeoCredential?.RedirectionUrl);

      return accessToken.AccessToken;
    }

    public async Task<string> GetUnauthenticatedToken()
    {
      AccessTokenResponse accessToken = await _authorizationClient.GetUnauthenticatedTokenAsync();

      return accessToken.AccessToken;
    }

    private string GetFullAccessUrl()
    {
      return _vimeoClient.GetOauthUrl(_vimeoCredential.RedirectionUrl, new List<string>
      {
        PUBLIC_SCOPE,
        PRIVATE_SCOPE,
        PURCHASED_SCOPE,
        CREATE_SCOPE,
        EDIT_SCOPE,
        DELETE_SCOPE,
        INTERACT_SCOPE,
        UPLOAD_SCOPE,
        PROMO_CODE_SCOPE,
        VIDEO_FILES_SCOPE
      }, _vimeoCredential.StateInformation);
    }

    private string GetReadAccessUrl()
    {
      return _vimeoClient.GetOauthUrl(_vimeoCredential.RedirectionUrl, new List<string>
      {
        PUBLIC_SCOPE,
      }, _vimeoCredential.StateInformation);
    }
  }
}
