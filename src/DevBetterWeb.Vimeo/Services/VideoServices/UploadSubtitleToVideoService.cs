using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Vimeo.Extensions;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class UploadSubtitleToVideoService : BaseAsyncApiCaller
    .WithRequest<UploadSubtitleToVideoRequest>
    .WithResponse<bool>
  {
    private readonly GetVideoService _getVideoService;
    private readonly GetUploadLinkTextTrackService _getUploadLinkTextTrackService;
    private readonly UploadTextTrackFileService _uploadTextTrackFileService;
    private readonly ActiveTextTrackService _activeTextTrackService;
    private readonly GetAllTextTracksService _getAllTextTracksService;
    private readonly ILogger<UploadSubtitleToVideoService> _logger;

    public UploadSubtitleToVideoService(
      GetVideoService getVideoService,
      GetUploadLinkTextTrackService getUploadLinkTextTrackService,
      UploadTextTrackFileService uploadTextTrackFileService,
      ActiveTextTrackService activeTextTrackService,
      GetAllTextTracksService getAllTextTracksService,
      ILogger<UploadSubtitleToVideoService> logger)
    {
      _getVideoService = getVideoService;
      _getUploadLinkTextTrackService = getUploadLinkTextTrackService;
      _uploadTextTrackFileService = uploadTextTrackFileService;
      _activeTextTrackService = activeTextTrackService;
      _getAllTextTracksService = getAllTextTracksService;
      _logger = logger;
    }

    public override async Task<HttpResponse<bool>> ExecuteAsync(UploadSubtitleToVideoRequest request, CancellationToken cancellationToken = default)
    {
      try
      {
        var videoResponse = await _getVideoService.ExecuteAsync(request.VideoId, cancellationToken);
        if (videoResponse.Code != System.Net.HttpStatusCode.OK)
        {
          return new HttpResponse<bool>(false, videoResponse.Code);
        }
        var video = videoResponse.Data;
        var textTracksUri = video?.Metadata?.Connections?.Texttracks?.Uri;

        var getUploadLinkTextTrackRequest = new GetUploadLinkTextTrackRequest(textTracksUri, Models.TextTrackType.TextTrackEnum.Subtitles);
        var getUploadLinkTextTrackresponse = await _getUploadLinkTextTrackService.ExecuteAsync(getUploadLinkTextTrackRequest, cancellationToken);
        if (getUploadLinkTextTrackresponse.Code != System.Net.HttpStatusCode.OK && getUploadLinkTextTrackresponse.Code != System.Net.HttpStatusCode.Created)
        {
          return new HttpResponse<bool>(false, getUploadLinkTextTrackresponse.Code);
        }

        var uploadTextTrackFileRequest = new UploadTextTrackFileRequest(getUploadLinkTextTrackresponse?.Data?.Link, request.VttFile);
        var uploadTextTrackFileResponse = await _uploadTextTrackFileService.ExecuteAsync(uploadTextTrackFileRequest);
        if (uploadTextTrackFileResponse.Code != System.Net.HttpStatusCode.OK)
        {
          return new HttpResponse<bool>(false, uploadTextTrackFileResponse.Code);
        }

        var textTracksResponse = await _getAllTextTracksService.ExecuteAsync(request.VideoId);
        if (textTracksResponse.Code != System.Net.HttpStatusCode.OK || textTracksResponse.Data == null || textTracksResponse.Data.Data == null || textTracksResponse.Data.Data.Count <= 0)
        {
          return new HttpResponse<bool>(false, textTracksResponse.Code);
        }

        var activeTextTrackRequest = new ActiveTextTrackRequest(textTracksResponse.Data.Data[textTracksResponse.Data.Data.Count-1].Uri);
        var activeTextTrackServiceResponse = await _activeTextTrackService.ExecuteAsync(activeTextTrackRequest);
        if (activeTextTrackServiceResponse.Code != System.Net.HttpStatusCode.OK)
        {
          return new HttpResponse<bool>(false, activeTextTrackServiceResponse.Code);
        }

        return new HttpResponse<bool>(true, System.Net.HttpStatusCode.OK);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception);
        return HttpResponse<bool>.FromException(exception.Message);
      }
    }
  }
}
