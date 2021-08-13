using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Vimeo.Extensions;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class UploadVideoService : BaseAsyncApiCaller
    .WithRequest<UploadVideoRequest>
    .WithResponse<long>
  {
    private readonly HttpService _httpService;
    private readonly ILogger<UploadVideoService> _logger;
    private readonly GetStreamingTicketService _getStreamingTicketService;
    private readonly CompleteUploadByCompleteUriService _completeUploadService;
    private readonly UpdateVideoDetailsService _updateVideoDetailsService;

    public UploadVideoService(HttpService httpService, ILogger<UploadVideoService> logger, GetStreamingTicketService getStreamingTicketService, CompleteUploadByCompleteUriService completeUploadService, UpdateVideoDetailsService updateVideoDetailsService)
    {
      _httpService = httpService;
      _logger = logger;
      _getStreamingTicketService = getStreamingTicketService;
      _completeUploadService = completeUploadService;
      _updateVideoDetailsService = updateVideoDetailsService;
    }

    public override async Task<HttpResponse<long>> ExecuteAsync(UploadVideoRequest request, CancellationToken cancellationToken = default)
    {
      try
      {
        var getStreamingTicketResponse = await _getStreamingTicketService.ExecuteAsync(cancellationToken);
        if (getStreamingTicketResponse?.Data == null)
        {
          return HttpResponse<long>.FromHttpResponseMessage(0, getStreamingTicketResponse.Code);
        }

        var uploadTicket = getStreamingTicketResponse.Data;
        if (string.IsNullOrEmpty(uploadTicket.CompleteUri) || string.IsNullOrEmpty(uploadTicket.UploadLink))
        {
          return HttpResponse<long>.FromHttpResponseMessage(0, getStreamingTicketResponse.Code);
        }

        var uploadResult = await UploadVideoDataAsync(uploadTicket.Upload.UploadLink, request.FileData);
        if (!uploadResult)
        {
          return HttpResponse<long>.FromHttpResponseMessage(0, getStreamingTicketResponse.Code);
        }
        var completeUploadRequest = new CompleteUploadRequest();
        completeUploadRequest.CompleteUri = uploadTicket.CompleteUri;
        var completeUploadResponse = await _completeUploadService.ExecuteAsync(completeUploadRequest, cancellationToken);
        if (completeUploadResponse.Data == 0)
        {
          return HttpResponse<long>.FromHttpResponseMessage(0, getStreamingTicketResponse.Code);
        }

        await _updateVideoDetailsService.ExecuteAsync(new UpdateVideoDetailsRequest(completeUploadResponse.Data, request.Video), cancellationToken);

        return HttpResponse<long>.FromHttpResponseMessage(completeUploadResponse.Data, completeUploadResponse.Code);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception);
        return HttpResponse<long>.FromException(exception.Message);
      }
    }

    public UploadVideoService SetToken(string token)
    {
      _httpService.SetAuthorization($"bearer {token}");

      return this;
    }

    private async Task<bool> UploadVideoDataAsync(string uploadUri, byte[] fileData)
    {
      _httpService.SetTimeout(60, TimeoutType.Minutes);

      var response = await _httpService.HttpPutBytesWithoutResponseAsync(uploadUri, fileData);

      _httpService.SetDefaultTimeout();

      return response;
    }
  }
}
