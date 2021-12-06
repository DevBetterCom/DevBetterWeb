using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Extensions;
using DevBetterWeb.Vimeo.Models;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class ActiveTextTrackService : BaseAsyncApiCaller
  .WithRequest<ActiveTextTrackRequest>
  .WithResponse<bool>
{
  private readonly HttpService _httpService;
  private readonly ILogger<ActiveTextTrackService> _logger;

  public ActiveTextTrackService(HttpService httpService, ILogger<ActiveTextTrackService> logger)
  {
    _httpService = httpService;
    _logger = logger;
  }

  public override async Task<HttpResponse<bool>> ExecuteAsync(ActiveTextTrackRequest request, CancellationToken cancellationToken = default)
  {
    try
    {
      var activeTextTrack = new ActiveTextTrack(request.Active);
      var response = await _httpService.HttpPatchWithoutResponseAsync(request.Uri, activeTextTrack);

      return response;
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
      return HttpResponse<bool>.FromException(exception.Message);
    }
  }
}
