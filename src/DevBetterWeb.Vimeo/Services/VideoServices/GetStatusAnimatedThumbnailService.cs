using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Vimeo.Extensions;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class GetStatusAnimatedThumbnailService : BaseAsyncApiCaller
    .WithRequest<GetAnimatedThumbnailRequest>
    .WithResponse<AnimatedThumbnailsResponse>
  {
    private readonly HttpService _httpService;
    private readonly ILogger<GetStatusAnimatedThumbnailService> _logger;

    public GetStatusAnimatedThumbnailService(HttpService httpService, ILogger<GetStatusAnimatedThumbnailService> logger)
    {
      _httpService = httpService;
      _logger = logger;
    }

    public override async Task<HttpResponse<AnimatedThumbnailsResponse>> ExecuteAsync(GetAnimatedThumbnailRequest request, CancellationToken cancellationToken = default)
    {
      var uri = $"videos/{request.VideoId}/animated_thumbsets/{request.PictureId}/status";
      try
      {
        var response = await _httpService.HttpGetAsync<AnimatedThumbnailsResponse>(uri);

        return response;
      }
      catch (Exception exception)
      {
        _logger.LogError(exception);
        return HttpResponse<AnimatedThumbnailsResponse>.FromException(exception.Message);
      }
    }
  }
}
