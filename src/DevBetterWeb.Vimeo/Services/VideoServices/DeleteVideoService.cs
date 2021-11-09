using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Extensions;
using DevBetterWeb.Vimeo.Models;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class DeleteVideoService : BaseAsyncApiCaller
  .WithRequest<string>
  .WithoutResponse
{
  private readonly HttpService _httpService;
  private readonly ILogger<DeleteVideoService> _logger;

  public DeleteVideoService(HttpService httpService, ILogger<DeleteVideoService> logger)
  {
    _httpService = httpService;
    _logger = logger;
  }

  public override async Task<HttpResponse> ExecuteAsync(string videoId, CancellationToken cancellationToken = default)
  {
    var uri = $"videos/{videoId}";
    try
    {
      var response = await _httpService.HttpDeleteAsync(uri);

      return HttpResponse.FromHttpResponseMessage(response.Code);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);

      return HttpResponse.FromException(exception.Message);
    }
  }
}
