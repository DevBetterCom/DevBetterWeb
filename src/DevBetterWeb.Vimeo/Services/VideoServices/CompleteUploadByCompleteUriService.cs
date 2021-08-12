using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Models;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Vimeo.Extensions;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class CompleteUploadByCompleteUriService : BaseAsyncApiCaller
    .WithRequest<CompleteUploadRequest>
    .WithResponse<long>
  {
    private readonly HttpService _httpService;
    private readonly ILogger<CompleteUploadByCompleteUriService> _logger;

    public CompleteUploadByCompleteUriService(HttpService httpService, ILogger<CompleteUploadByCompleteUriService> logger)
    {
      _httpService = httpService;
      _logger = logger;
    }

    public override async Task<HttpResponse<long>> ExecuteAsync(CompleteUploadRequest request, CancellationToken cancellationToken = default)
    {
      try
      {
        var response = await _httpService.HttpDeleteAsync(request.CompleteUri);

        return HttpResponse<long>.FromHttpResponseMessage(ParseVideoId(response), response.Code);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception);
        return HttpResponse<long>.FromException(exception.Message);
      }
    }

    public CompleteUploadByCompleteUriService SetToken(string token)
    {
      _httpService.SetAuthorization($"bearer {token}");

      return this;
    }

    private long ParseVideoId(HttpResponse<bool> response)
    {
      var headerLocation = response.GetFirstHeader("Location");
      if (string.IsNullOrEmpty(headerLocation))
      {
        response.SetData(false);
        return 0;
      }

      var parts = headerLocation.Split("/");
      if (parts.Length <= 0)
      {
        response.SetData(false);
        return 0;
      }

      var videoId = long.Parse(parts[parts.Length - 1]);

      return videoId;
    }
  }
}
