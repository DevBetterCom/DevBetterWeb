using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Extensions;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class GetAllAnimatedThumbnailService : BaseAsyncApiCaller
  .WithRequest<GetAnimatedThumbnailRequest>
  .WithResponse<GetAllAnimatedThumbnailResponse>
{
  private readonly HttpService _httpService;
  private readonly ILogger<GetAllAnimatedThumbnailService> _logger;

  public GetAllAnimatedThumbnailService(HttpService httpService, ILogger<GetAllAnimatedThumbnailService> logger)
  {
    _httpService = httpService;
    _logger = logger;
  }

  public override async Task<HttpResponse<GetAllAnimatedThumbnailResponse>> ExecuteAsync(GetAnimatedThumbnailRequest request, CancellationToken cancellationToken = default)
  {
    var uri = $"videos/{request.VideoId}/animated_thumbsets";
    try
    {
      var response = await _httpService.HttpGetAsync<GetAllAnimatedThumbnailResponse>(uri, cancellationToken);

      return response;
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
      return HttpResponse<GetAllAnimatedThumbnailResponse>.FromException(exception.Message);
    }
  }
}
