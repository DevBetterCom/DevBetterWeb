using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.WpfUploader.Models;

namespace DevBetterWeb.WpfUploader.ApiClients;

public class UpdateAllVideosThumbnails : BaseAsyncApiCaller
  .WithoutRequest
  .WithoutResponse
{
  private readonly HttpService _httpService;

  public UpdateAllVideosThumbnails(HttpService httpService)
  {
    _httpService = httpService;
  }

  public override async Task<HttpResponse> ExecuteAsync(CancellationToken cancellationToken = default)
  {
    var uri = $"videos/update-all-videos-thumbnails";
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
