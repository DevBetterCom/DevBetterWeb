using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Extensions;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class AddDomainToVideoService : BaseAsyncApiCaller
  .WithRequest<AddDomainToVideoRequest>
  .WithResponse<bool>
{
  private readonly HttpService _httpService;
  private readonly ILogger<AddDomainToVideoService> _logger;

  public AddDomainToVideoService(HttpService httpService, ILogger<AddDomainToVideoService> logger)
  {
    _httpService = httpService;
    _logger = logger;
  }

  public override async Task<HttpResponse<bool>> ExecuteAsync(AddDomainToVideoRequest request, CancellationToken cancellationToken = default)
  {
    var uri = $"videos/{request.VideoId}/privacy/domains/{request.Domain}";
    try
    {
      var response = await _httpService.HttpPutBytesAsync(uri, null, cancellationToken);

      return response;
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
      return HttpResponse<bool>.FromException(exception.Message);
    }
  }
}
