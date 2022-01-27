using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;

namespace DevBetterWeb.UploaderApp;

public class UpdateVideoThumbnails : BaseAsyncApiCaller
  .WithRequest<ArchiveVideo>
  .WithoutResponse
{
  private readonly HttpService _httpService;

  public UpdateVideoThumbnails(HttpService httpService)
  {
    _httpService = httpService;
  }

  public override async Task<HttpResponse> ExecuteAsync(ArchiveVideo archiveVideo, CancellationToken cancellationToken = default)
  {
    var uri = $"videos/update-video-thumbnails";
    try
    {
      var response = await _httpService.HttpPostAsync<ArchiveVideo>(uri, archiveVideo);

      return HttpResponse.FromHttpResponseMessage(response.Code);
    }
    catch (Exception exception)
    {
      return HttpResponse.FromException(exception.Message);
    }
  }
}
