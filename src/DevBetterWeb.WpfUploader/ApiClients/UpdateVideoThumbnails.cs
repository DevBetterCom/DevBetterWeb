using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.WpfUploader.Models;

namespace DevBetterWeb.WpfUploader.ApiClients;

public class UpdateVideoThumbnails : BaseAsyncApiCaller
  .WithRequest<string>
  .WithoutResponse
{
  private readonly HttpService _httpService;

  public UpdateVideoThumbnails(HttpService httpService)
  {
    _httpService = httpService;
  }

  public override async Task<HttpResponse> ExecuteAsync(string videoId, CancellationToken cancellationToken = default)
  {
    var uri = $"videos/update-video-thumbnails/{videoId}";
    try
    {
      var response = await _httpService.HttpGetAsync<ArchiveVideo>(uri);

      return HttpResponse.FromHttpResponseMessage(response.Code);
    }
    catch (Exception exception)
    {
      return HttpResponse.FromException(exception.Message);
    }
  }
}
