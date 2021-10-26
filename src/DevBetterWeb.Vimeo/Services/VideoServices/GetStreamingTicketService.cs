using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Vimeo.Extensions;
using DevBetterWeb.Vimeo.Models;
using System.Collections.Generic;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class GetStreamingTicketService : BaseAsyncApiCaller
    .WithoutRequest
    .WithResponse<UploadTicket>
  {
    private readonly HttpService _httpService;
    private readonly ILogger<GetStreamingTicketService> _logger;

    public GetStreamingTicketService(HttpService httpService, ILogger<GetStreamingTicketService> logger)
    {
      _httpService = httpService;
      _logger = logger;
    }

    public override async Task<HttpResponse<UploadTicket>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
      var uri = $"me/videos";
      try
      {
        var query = new Dictionary<string, string>();

        query.AddIfNotNull("type", UploadVideoType.STREAMING?.ToString());

        var response = await _httpService.HttpPostByQueryAsync<UploadTicket>(uri, query);

        return response;
      }
      catch (Exception exception)
      {
        _logger.LogError(exception);
        return HttpResponse<UploadTicket>.FromException(exception.Message);
      }
    }
  }
}
