using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;

namespace DevBetterWeb.WpfUploader.ApiClients;

public class DeleteVideo : BaseAsyncApiCaller
  .WithRequest<string>
  .WithoutResponse
{
  private readonly HttpService _httpService;

  public DeleteVideo(HttpService httpService)
  {
    _httpService = httpService;
  }

  public override async Task<HttpResponse> ExecuteAsync(string vimeoVideoId, CancellationToken cancellationToken = default)
  {
    var uri = $"videos/uploader/delete-video/{vimeoVideoId}";
    try
    {
      var response = await _httpService.HttpDeleteAsync(uri);

      return HttpResponse.FromHttpResponseMessage(response.Code);
    }
    catch (Exception exception)
    {
      return HttpResponse.FromException(exception.Message);
    }
  }
}
