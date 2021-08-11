using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using VimeoDotNet;
using VimeoDotNet.Authorization;
using VimeoDotNet.Enums;
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
    private readonly string _accessToken = string.Empty;

    public VimeoService(VimeoCredential vimeoCredential)
    {
      _vimeoCredential = vimeoCredential;
      if (string.IsNullOrEmpty(vimeoCredential.AccessToken))
      {
        _vimeoClient = new VimeoClient(_vimeoCredential.ClientId, _vimeoCredential.ClientSecret);
      }
      else
      {
        _accessToken = vimeoCredential.AccessToken;
        _vimeoClient = new VimeoClient(vimeoCredential.AccessToken);
      }
      
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

    public async Task<Paginated<Video>> GetAllVideosAsync()
    {
      var videos = await _vimeoClient.GetVideosAsync(UserId.Me, 1, 10);
      return videos;
    }
    public async Task<bool> DeleteAllVideosAsync()
    {
      var videos = await GetAllVideosAsync();
      foreach (var video in videos.Data)
      {
        if (video?.Id == null)
        {
          continue;
        }
        await _vimeoClient.DeleteVideoAsync(video.Id.Value);
      }

      return true;
    }



    public async Task<User> GetAccountInformationAsync()
    {
      var user = await _vimeoClient.GetAccountInformationAsync();

      return user;
    }

    public async Task UpdateVideoDetails(long videoId, VideoUpdateMetadata videoMetadata)
    {
      await _vimeoClient.UpdateVideoMetadataAsync(videoId, videoMetadata);
    }

    public async Task<bool> UploadVideoAsync(string videoName, byte[] fileData)
    {
      var uploadTicket = await GetUploadTicketAsync();
      if (string.IsNullOrEmpty(uploadTicket?.CompleteUri) || string.IsNullOrEmpty(uploadTicket.UploadLinkSecure))
      {
        return false;
      }

      var uploadResult = await UploadVideoDataAsync(uploadTicket.UploadLinkSecure, fileData);
      if (!uploadResult)
      {
        return false;
      }
      var videoId =  await DeleteUploadTicketAsync(uploadTicket.CompleteUri);
      if (videoId == null)
      {
        return false;
      }

      var videoDetails = new VideoUpdateMetadata();
      videoDetails.Name = videoName;
      videoDetails.Privacy = VideoPrivacyEnum.Password;
      videoDetails.Password = "122324";
      videoDetails.AllowDownloadVideo = false;
      await UpdateVideoDetails(videoId.Value, videoDetails);

      return true;
    }

    private long ParseVideoId(string completeUri)
    {
      var parts = completeUri.Split("?");
      if (parts.Length <= 1)
      {
        return 0;
      }

      var items = HttpUtility.ParseQueryString(parts[1]);
      var videoId = items["video_file_id"];
      if (videoId == null)
      {
        return 0;
      }

      return long.Parse(videoId);
    }

    public async Task<long?> DeleteUploadTicketAsync(string completeUri)
    {
      var httpClient = new HttpClient();
      httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {_accessToken}");

      var response = await httpClient.DeleteAsync($"https://api.vimeo.com{completeUri}");
      if (!response.IsSuccessStatusCode)
      {
        return null;
      }
      var contents = await response.Content.ReadAsStringAsync();
      if (response.Headers.Location == null)
      {
        return null;
      }
      var headerLocation = response.Headers.GetValues("Location").FirstOrDefault();
      if (string.IsNullOrEmpty(headerLocation))
      {
        return null;
      }

      var parts = headerLocation.Split("/");
      if (parts.Length <= 0)
      {
        return null;
      }

      return long.Parse(parts[parts.Length - 1]);
    }

    public async Task<UploadTicket> GetUploadTicketAsync()
    {
      var uploadTicket = await _vimeoClient.GetUploadTicketAsync();

      return uploadTicket;
    }

    private async Task<bool> UploadVideoDataAsync(string uploadUri, byte[] fileData)
    {
      var httpClient = new HttpClient();
      httpClient.Timeout = TimeSpan.FromMinutes(60);

      httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {_accessToken}");
      //httpClient.DefaultRequestHeaders
      //  .Accept
      //  .Add(new MediaTypeWithQualityHeaderValue("mp4"));
      //httpClient.DefaultRequestHeaders.Add("Content-Length", $"{fileData.Length}");

      var byteContent = new ByteArrayContent(fileData);
      //var multipartContent = new MultipartFormDataContent();
      //multipartContent.Add(byteContent, "video");

      var response = await httpClient.PutAsync(uploadUri, byteContent);
      if (!response.IsSuccessStatusCode)
      {
        return false;
      }
      var contents = await response.Content.ReadAsStringAsync();

      return true;
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
