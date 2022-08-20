using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Extensions;
using DevBetterWeb.Vimeo.Models;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

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
		var query = new Dictionary<string, string>();
		query.AddIfNotNull("type", UploadVideoType.STREAMING?.ToString());
		try
		{

      var response = await _httpService.HttpPostByQueryAsync<UploadTicket>(uri, query, cancellationToken);

      return response;
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Details: uri, querytype {0}, {1}", uri, UploadVideoType.STREAMING?.ToString());
      return HttpResponse<UploadTicket>.FromException(exception.Message);
    }
  }
}
