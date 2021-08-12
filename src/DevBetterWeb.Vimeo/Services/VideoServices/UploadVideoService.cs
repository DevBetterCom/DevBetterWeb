using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Models;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Vimeo.Extensions;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class UploadVideoService : BaseAsyncApiCaller
    .WithRequest<UploadVideoRequest>
    .WithResponse<long>
  {
    private readonly HttpService _httpService;
    private readonly ILogger<Upload> _logger;
    private readonly UploadAttemptService _uploadAttemptService;
    private readonly CompleteUploadService _completeUploadService;
    private readonly UpdateVideoDetailsService _updateVideoDetailsService;

    public UploadVideoService(HttpService httpService, ILogger<Upload> logger, UploadAttemptService uploadAttemptService, CompleteUploadService completeUploadService, UpdateVideoDetailsService updateVideoDetailsService)
    {
      _httpService = httpService;
      _logger = logger;
      _uploadAttemptService = uploadAttemptService;
      _completeUploadService = completeUploadService;
      _updateVideoDetailsService = updateVideoDetailsService;
    }

    public override async Task<HttpResponse<long>> ExecuteAsync(UploadVideoRequest request, CancellationToken cancellationToken = default)
    {
      try
      {
        var uploadAttemptResponse = await _uploadAttemptService.ExecuteAsync(request.UserId);
        if (uploadAttemptResponse?.Data == null)
        {
          return HttpResponse<long>.FromHttpResponseMessage(0, uploadAttemptResponse.Code);
        }

        var uploadAttempt = uploadAttemptResponse.Data;
        if (string.IsNullOrEmpty(uploadAttempt.CompleteUri) || string.IsNullOrEmpty(uploadAttempt.UploadLink))
        {
          return HttpResponse<long>.FromHttpResponseMessage(0, uploadAttemptResponse.Code);
        }

        var uploadResult = await UploadVideoDataAsync(uploadAttempt.UploadLink, request.FileData);
        if (!uploadResult)
        {
          return HttpResponse<long>.FromHttpResponseMessage(0, uploadAttemptResponse.Code);
        }
        var completeUploadResponse = await _completeUploadService.ExecuteAsync(uploadAttempt.CompleteUri);
        if (completeUploadResponse.Data == 0)
        {
          return HttpResponse<long>.FromHttpResponseMessage(0, uploadAttemptResponse.Code);
        }

        var videoId = completeUploadResponse.Data;
        await _updateVideoDetailsService.ExecuteAsync(new UpdateVideoDetailsRequest(videoId, request.Video), cancellationToken);

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
      _httpService.SetAuthorization(token);

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
