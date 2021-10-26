using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Vimeo.Extensions;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class CompleteUploadService : BaseAsyncApiCaller
    .WithRequest<CompleteUploadRequest>
    .WithResponse<long>
  {
    private readonly HttpService _httpService;
    private readonly ILogger<CompleteUploadService> _logger;

    public CompleteUploadService(HttpService httpService, ILogger<CompleteUploadService> logger)
    {
      _httpService = httpService;
      _logger = logger;
    }

    public override async Task<HttpResponse<long>> ExecuteAsync(CompleteUploadRequest request, CancellationToken cancellationToken = default)
    {
      var fullUri = $"users";
      try
      {
        var response = await _httpService.HttpDeleteAsync($"{fullUri}/{request.UserId}/uploads/{request.UploadId}");

        return HttpResponse<long>.FromHttpResponseMessage(ParseVideoId(response), response.Code);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception);
        return HttpResponse<long>.FromException(exception.Message);
      }
    }

    public long ParseVideoId(HttpResponse<bool> response)
    {
      var headerLocation = response.GetFirstHeader("Location");
      if (string.IsNullOrEmpty(headerLocation))
      {
        return 0;
      }

      var parts = headerLocation.Split("/");
      if (parts.Length <= 0)
      {
        return 0;
      }

      var videoId = long.Parse(parts[parts.Length - 1]);

      return videoId;
    }
  }
}
