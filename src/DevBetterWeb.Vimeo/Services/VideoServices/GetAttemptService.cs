using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Helpers;
using DevBetterWeb.Vimeo.Models;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Vimeo.Extensions;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class GetAttemptService : BaseAsyncApiCaller
    .WithRequest<GetAttemptRequest>
    .WithResponse<UploadAttempt>
  {
    private readonly HttpService _httpService;
    private readonly ILogger<GetAttemptService> _logger;

    public GetAttemptService(HttpService httpService, ILogger<GetAttemptService> logger)
    {
      _httpService = httpService;
      _logger = logger;
    }

    public override async Task<HttpResponse<UploadAttempt>> ExecuteAsync(GetAttemptRequest request, CancellationToken cancellationToken = default)
    {
      var uri = $"users";
      try
      {
        var uploadId = new RandomHelper().CreateNumber(10000, 99999);
        var uploadAttempt = await _httpService.HttpGetAsync<UploadAttempt>($"{uri}/{request.UserId}/uploads/{request.UploadId}");

        return uploadAttempt;
      }
      catch (Exception exception)
      {
        _logger.LogError(exception);
        return HttpResponse<UploadAttempt>.FromException(exception.Message);
      }
    }
  }
}
