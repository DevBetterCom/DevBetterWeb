using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Extensions;
using DevBetterWeb.Vimeo.Models;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class GetDomainsVideoService : BaseAsyncApiCaller
  .WithRequest<string>
  .WithResponse<Domain>
{
  private readonly HttpService _httpService;
  private readonly ILogger<GetDomainsVideoService> _logger;

  public GetDomainsVideoService(HttpService httpService, ILogger<GetDomainsVideoService> logger)
  {
    _httpService = httpService;
    _logger = logger;
  }

  public override async Task<HttpResponse<Domain>> ExecuteAsync(string videoId, CancellationToken cancellationToken = default)
  {
    var uri = $"videos/{videoId}/privacy/domains?page=1&per_page=100";
    try
    {
      var response = await _httpService.HttpGetAsync<Domain>(uri, cancellationToken);

      return response;
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
      return HttpResponse<Domain>.FromException(exception.Message);
    }
  }
}
