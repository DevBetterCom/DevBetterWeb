using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Vimeo.Extensions;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class GetAnimatedThumbnailService : BaseAsyncApiCaller
    .WithRequest<GetAnimatedThumbnailRequest>
    .WithResponse<AnimatedThumbnailsResponse>
  {
    private readonly HttpService _httpService;
    private readonly ILogger<GetAnimatedThumbnailService> _logger;

    public GetAnimatedThumbnailService(HttpService httpService, ILogger<GetAnimatedThumbnailService> logger)
    {
      _httpService = httpService;
      _logger = logger;
    }

    public override async Task<HttpResponse<AnimatedThumbnailsResponse>> ExecuteAsync(GetAnimatedThumbnailRequest request, CancellationToken cancellationToken = default)
    {
      var uri = $"videos/{request.VideoId}/animated_thumbsets/{request.PictureId}";
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
