using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Constants;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Vimeo.Extensions;
using DevBetterWeb.Vimeo.Models;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
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

    public GetOEmbedVideoService SetToken(string token)
    {
      _httpService.SetAuthorization($"bearer {token}");

      return this;
    }
  }
}
