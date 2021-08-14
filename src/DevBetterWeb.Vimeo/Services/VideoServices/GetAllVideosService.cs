using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Vimeo.Extensions;
using DevBetterWeb.Vimeo.Models;
using System.Collections.Generic;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class GetAllVideosService : BaseAsyncApiCaller
    .WithRequest<GetAllVideosRequest>
    .WithResponse<List<Video>>
  {
    private readonly HttpService _httpService;
    private readonly ILogger<GetAllVideosService> _logger;

    public GetAllVideosService(HttpService httpService, ILogger<GetAllVideosService> logger)
    {
      _httpService = httpService;
      _logger = logger;
    }

    public override async Task<HttpResponse<List<Video>>> ExecuteAsync(GetAllVideosRequest request, CancellationToken cancellationToken = default)
    {
      var uri = $"users/{request.UserId}/videos";
      try
      {
        var query = new QueryBuilder()
          .Add("page", request.Page)
          .Add("per_page", request.PageSize);


        var response = await _httpService.HttpGetAsync<List<Video>>($"{uri}", query);

        return response;
      }
      catch (Exception exception)
      {
        _logger.LogError(exception);
        return HttpResponse<List<Video>>.FromException(exception.Message);
      }
    }

    public GetAllVideosService SetToken(string token)
    {
      _httpService.SetAuthorization($"bearer {token}");

      return this;
    }
  }
}
