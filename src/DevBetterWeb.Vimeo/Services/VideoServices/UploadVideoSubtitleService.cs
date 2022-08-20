using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Extensions;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class UploadVideoSubtitleService : BaseAsyncApiCaller
  .WithRequest<UploadVideoSubtitleRequest>
  .WithResponse<bool>
{
  private readonly GetVideoService _getVideoService;
  private readonly GetUploadLinkTextTrackService _getUploadLinkTextTrackService;
  private readonly UploadTextTrackFileService _uploadTextTrackFileService;
  private readonly ActiveTextTrackService _activeTextTrackService;
  private readonly GetAllTextTracksService _getAllTextTracksService;
  private readonly ILogger<UploadVideoSubtitleService> _logger;

  public UploadVideoSubtitleService(
    GetVideoService getVideoService,
    GetUploadLinkTextTrackService getUploadLinkTextTrackService,
    UploadTextTrackFileService uploadTextTrackFileService,
    ActiveTextTrackService activeTextTrackService,
    GetAllTextTracksService getAllTextTracksService,
    ILogger<UploadVideoSubtitleService> logger)
  {
    _getVideoService = getVideoService;
    _getUploadLinkTextTrackService = getUploadLinkTextTrackService;
    _uploadTextTrackFileService = uploadTextTrackFileService;
    _activeTextTrackService = activeTextTrackService;
    _getAllTextTracksService = getAllTextTracksService;
    _logger = logger;
  }

  public override async Task<HttpResponse<bool>> ExecuteAsync(UploadVideoSubtitleRequest request, CancellationToken cancellationToken = default)
  {
    try
    {
      var videoResponse = await _getVideoService.ExecuteAsync(request.VideoId, cancellationToken);
      if (videoResponse == null)
      {
        return new HttpResponse<bool>(false, HttpStatusCode.InternalServerError);
      }
      if (videoResponse.Code != System.Net.HttpStatusCode.OK)
      {
        return new HttpResponse<bool>(false, videoResponse.Code);
      }
      var video = videoResponse.Data;
      var textTracksUri = video?.Metadata?.Connections?.Texttracks?.Uri;

      var getUploadLinkTextTrackRequest = new GetUploadLinkTextTrackRequest(textTracksUri, Models.TextTrackType.TextTrackEnum.Subtitles);
      var getUploadLinkTextTrackResponse = await _getUploadLinkTextTrackService.ExecuteAsync(getUploadLinkTextTrackRequest, cancellationToken);
      if (getUploadLinkTextTrackResponse!.Code != HttpStatusCode.OK && getUploadLinkTextTrackResponse!.Code != HttpStatusCode.Created)
      {
        return new HttpResponse<bool>(false, getUploadLinkTextTrackResponse.Code);
      }

      var uploadTextTrackFileRequest = new UploadTextTrackFileRequest(getUploadLinkTextTrackResponse?.Data?.Link, request.SubtitleFile);
      var uploadTextTrackFileResponse = await _uploadTextTrackFileService.ExecuteAsync(uploadTextTrackFileRequest, cancellationToken);
      if (uploadTextTrackFileResponse.Code != System.Net.HttpStatusCode.OK)
      {
        return new HttpResponse<bool>(false, uploadTextTrackFileResponse.Code);
      }

      var textTracksResponse = await _getAllTextTracksService.ExecuteAsync(request.VideoId, cancellationToken);
      if (textTracksResponse.Code != System.Net.HttpStatusCode.OK || textTracksResponse.Data?.Data == null || textTracksResponse.Data.Data.Count <= 0)
      {
        return new HttpResponse<bool>(false, textTracksResponse.Code);
      }

      var activeTextTrackRequest = new ActiveTextTrackRequest(textTracksResponse.Data.Data[^1].Uri);
      var activeTextTrackServiceResponse = await _activeTextTrackService.ExecuteAsync(activeTextTrackRequest, cancellationToken);
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
