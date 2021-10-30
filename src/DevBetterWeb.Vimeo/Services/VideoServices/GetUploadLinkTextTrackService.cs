using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Vimeo.Extensions;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class GetUploadLinkTextTrackService : BaseAsyncApiCaller
    .WithRequest<GetUploadLinkTextTrackRequest>
    .WithResponse<GetUploadLinkTextTrackResponse>
  {
    private readonly HttpService _httpService;
    private readonly ILogger<AddAnimatedThumbnailsToVideoService> _logger;

    public GetUploadLinkTextTrackService(HttpService httpService, ILogger<AddAnimatedThumbnailsToVideoService> logger)
    {
      _httpService = httpService;
      _logger = logger;
    }

    public override async Task<HttpResponse<GetUploadLinkTextTrackResponse>> ExecuteAsync(GetUploadLinkTextTrackRequest request, CancellationToken cancellationToken = default)
    {
      var uri = request.Uri;
      try
      {
        var response = await _httpService.HttpPostAsync<GetUploadLinkTextTrackResponse>(uri, request);

        return response;
      }
      catch (Exception exception)
      {
        _logger.LogError(exception);
        return HttpResponse<GetUploadLinkTextTrackResponse>.FromException(exception.Message);
      }
    }
  }
}
