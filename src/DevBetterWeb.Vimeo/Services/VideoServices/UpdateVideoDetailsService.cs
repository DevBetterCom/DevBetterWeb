using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Extensions;
using DevBetterWeb.Vimeo.Models;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class UpdateVideoDetailsService : BaseAsyncApiCaller
  .WithRequest<UpdateVideoDetailsRequest>
  .WithResponse<bool>
{
  private readonly HttpService _httpService;
  private readonly ILogger<UpdateVideoDetailsService> _logger;

  public UpdateVideoDetailsService(HttpService httpService, ILogger<UpdateVideoDetailsService> logger)
  {
    _httpService = httpService;
    _logger = logger;
  }

  public override async Task<HttpResponse<bool>> ExecuteAsync(UpdateVideoDetailsRequest request, CancellationToken cancellationToken = default)
  {
    var uri = $"videos/{request.VideoId}";
    try
    {
      var response = await _httpService.HttpPatchWithoutResponseAsync(uri, request.Video);

      return response;
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
      return HttpResponse<bool>.FromException(exception.Message);
    }
  }
}
