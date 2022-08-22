using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Extensions;
using DevBetterWeb.Vimeo.Models;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class GetAllTextTracksService : BaseAsyncApiCaller
  .WithRequest<string>
  .WithResponse<GetAllTextTracksResponse>
{
  private readonly HttpService _httpService;
  private readonly ILogger<GetAllTextTracksService> _logger;

  public GetAllTextTracksService(HttpService httpService, ILogger<GetAllTextTracksService> logger)
  {
    _httpService = httpService;
    _logger = logger;
  }

  public override async Task<HttpResponse<GetAllTextTracksResponse>> ExecuteAsync(string videoId, CancellationToken cancellationToken = default)
  {
    var uri = $"videos/{videoId}/texttracks";
    try
    {
      var response = await _httpService.HttpGetAsync<GetAllTextTracksResponse>(uri, cancellationToken);

      return response;
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
      return HttpResponse<GetAllTextTracksResponse>.FromException(exception.Message);
    }
  }
}
