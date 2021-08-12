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
  public class UploadAttemptService : BaseAsyncApiCaller
    .WithRequest<string>
    .WithResponse<UploadAttempt>
  {
    private readonly HttpService _httpService;
    private readonly ILogger<UploadAttemptService> _logger;

    public UploadAttemptService(HttpService httpService, ILogger<UploadAttemptService> logger)
    {
      _httpService = httpService;
      _logger = logger;
    }

    public override async Task<HttpResponse<UploadAttempt>> ExecuteAsync(string userId, CancellationToken cancellationToken = default)
    {
      var fullUri = $"/users";
      try
      {
        var uploadId = new RandomHelper().CreateNumber(10000, 99999);
        var uploadAttempt = await _httpService.HttpGetAsync<UploadAttempt>($"{fullUri}/{userId}/uploads/{uploadId}");

        return uploadAttempt;
      }
      catch (Exception exception)
      {
        _logger.LogError(exception);
        return HttpResponse<UploadAttempt>.FromException(exception.Message);
      }
    }

    public UploadAttemptService SetToken(string token)
    {
      _httpService.SetAuthorization($"bearer {token}");

      return this;
    }
  }
}
