using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Extensions;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class UploadTextTrackFileService : BaseAsyncApiCaller
  .WithRequest<UploadTextTrackFileRequest>
  .WithResponse<bool>
{
  private readonly HttpService _httpService;
  private readonly ILogger<UploadTextTrackFileService> _logger;

  public UploadTextTrackFileService(HttpService httpService, ILogger<UploadTextTrackFileService> logger)
  {
    _httpService = httpService;
    _logger = logger;
  }

  public override async Task<HttpResponse<bool>> ExecuteAsync(UploadTextTrackFileRequest request, CancellationToken cancellationToken = default)
  {
    var uri = request.Link;
    try
    {
      _httpService.ResetBaseUri();
      var response = await _httpService.HttpPutBytesWithoutResponseAsync(uri, request.FileData);
      _httpService.ResetHttp(ServiceConstants.VIMEO_URI);

      if (!response)
      {
        return new HttpResponse<bool>(false, System.Net.HttpStatusCode.NotFound);
      }
      return new HttpResponse<bool>(true, System.Net.HttpStatusCode.OK);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
      return HttpResponse<bool>.FromException(exception.Message);
    }
  }
}
