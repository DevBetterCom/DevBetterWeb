using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Extensions;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class AddAnimatedThumbnailsToVideoService : BaseAsyncApiCaller
  .WithRequest<AddAnimatedThumbnailsToVideoRequest>
  .WithResponse<AnimatedThumbnailsResponse>
{
  private readonly HttpService _httpService;
  private readonly ILogger<AddAnimatedThumbnailsToVideoService> _logger;

  public AddAnimatedThumbnailsToVideoService(HttpService httpService, ILogger<AddAnimatedThumbnailsToVideoService> logger)
  {
    _httpService = httpService;
    _logger = logger;
  }

  public override async Task<HttpResponse<AnimatedThumbnailsResponse>> ExecuteAsync(AddAnimatedThumbnailsToVideoRequest request, CancellationToken cancellationToken = default)
  {
    var uri = $"videos/{request.VideoId}/animated_thumbsets";
    try
    {
      var response = await _httpService.HttpPostAsync<AnimatedThumbnailsResponse>(uri, request);

      return response;
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
      return HttpResponse<AnimatedThumbnailsResponse>.FromException(exception.Message);
    }
  }
}
