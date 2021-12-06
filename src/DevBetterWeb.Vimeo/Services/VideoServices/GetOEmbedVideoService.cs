using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Extensions;
using DevBetterWeb.Vimeo.Models;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class GetOEmbedVideoService : BaseAsyncApiCaller
  .WithRequest<string>
  .WithResponse<OEmbed>
{
  private readonly HttpService _httpService;
  private readonly ILogger<GetOEmbedVideoService> _logger;

  public GetOEmbedVideoService(HttpService httpService, ILogger<GetOEmbedVideoService> logger)
  {
    _httpService = httpService;
    _logger = logger;
  }

  public override async Task<HttpResponse<OEmbed>> ExecuteAsync(string link, CancellationToken cancellationToken = default)
  {
    var uri = $"https://vimeo.com/api/oembed.json";
    try
    {
      _httpService.ResetBaseUri();
      var response = await _httpService.HttpGetAsync<OEmbed>($"{uri}?url={link}");
      _httpService.ResetHttp(ServiceConstants.VIMEO_URI);

      return response;
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
      return HttpResponse<OEmbed>.FromException(exception.Message);
    }
  }
}
