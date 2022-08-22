using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Extensions;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class CompleteUploadByCompleteUriService : BaseAsyncApiCaller
  .WithRequest<CompleteUploadRequest>
  .WithResponse<long>
{
  private readonly HttpService _httpService;
  private readonly ILogger<CompleteUploadByCompleteUriService> _logger;

  public CompleteUploadByCompleteUriService(HttpService httpService, ILogger<CompleteUploadByCompleteUriService> logger)
  {
    _httpService = httpService;
    _logger = logger;
  }

  public override async Task<HttpResponse<long>> ExecuteAsync(CompleteUploadRequest request, CancellationToken cancellationToken = default)
  {
    try
    {
      var response = await _httpService.HttpDeleteAsync(request.CompleteUri, cancellationToken);

      return HttpResponse<long>.FromHttpResponseMessage(ParseVideoId(response), response.Code);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
      return HttpResponse<long>.FromException(exception.Message);
    }
  }

  private long ParseVideoId(HttpResponse<bool> response)
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
