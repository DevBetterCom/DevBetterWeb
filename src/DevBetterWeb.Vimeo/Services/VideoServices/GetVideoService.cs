using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Vimeo.Extensions;
using DevBetterWeb.Vimeo.Models;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class GetVideoService : BaseAsyncApiCaller
    .WithRequest<string>
    .WithResponse<Video>
  {
    private readonly HttpService _httpService;
    private readonly ILogger<GetVideoService> _logger;

    public GetVideoService(HttpService httpService, ILogger<GetVideoService> logger)
    {
      _httpService = httpService;
      _logger = logger;
    }

    public override async Task<HttpResponse<Video>> ExecuteAsync(string videoId, CancellationToken cancellationToken = default)
    {
      var uri = $"videos";
      try
      {
        var response = await _httpService.HttpGetAsync<Video>($"{uri}/{videoId}");

        if(response == null)
        {
          throw new Exception($"No video found for {uri}/{videoId}");
        }
        return response;
      }
      catch (Exception exception)
      {
        _logger.LogError(exception);
        return HttpResponse<Video>.FromException(exception.Message);
      }
    }
  }
}
