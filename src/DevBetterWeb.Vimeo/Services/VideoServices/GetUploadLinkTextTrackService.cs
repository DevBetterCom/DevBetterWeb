using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Extensions;
using DevBetterWeb.Vimeo.Models;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class GetUploadLinkTextTrackService : BaseAsyncApiCaller
  .WithRequest<GetUploadLinkTextTrackRequest>
  .WithResponse<GetUploadLinkTextTrackResponse>
{
  private readonly HttpService _httpService;
  private readonly ILogger<GetUploadLinkTextTrackService> _logger;

  public GetUploadLinkTextTrackService(HttpService httpService, ILogger<GetUploadLinkTextTrackService> logger)
  {
    _httpService = httpService;
    _logger = logger;
  }

  public override async Task<HttpResponse<GetUploadLinkTextTrackResponse>> ExecuteAsync(GetUploadLinkTextTrackRequest request, CancellationToken cancellationToken = default)
  {
    var uri = request.Uri;
    try
    {
      var texttack = new TextTrack();
      texttack.Name = request.Name;
      texttack.Language = request.Language;
      texttack.Type = request.Type;

      var response = await _httpService.HttpPostAsync<GetUploadLinkTextTrackResponse>(uri, texttack);

      return response;
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
      return HttpResponse<GetUploadLinkTextTrackResponse>.FromException(exception.Message);
    }
  }
}
