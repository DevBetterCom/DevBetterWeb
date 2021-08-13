using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Vimeo.Extensions;
using DevBetterWeb.Vimeo.Models;

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
        var query = new QueryBuilder()
          .Add("type", UploadVideoType.STREAMING);

        var response = await _httpService.HttpPostByQueryAsync<UploadTicket>(uri, query);

        return response;
      }
      catch (Exception exception)
      {
        _logger.LogError(exception);
        return HttpResponse<UploadTicket>.FromException(exception.Message);
      }
    }

    public GetStreamingTicketService SetToken(string token)
    {
      _httpService.SetAuthorization($"bearer {token}");

      return this;
    }
  }
}
