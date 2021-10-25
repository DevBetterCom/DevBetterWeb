using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Vimeo.Extensions;
using DevBetterWeb.Vimeo.Models;
using System.Collections.Generic;
using DevBetterWeb.Vimeo.Constants;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class GetAllVideosService : BaseAsyncApiCaller
    .WithRequest<GetAllVideosRequest>
    .WithResponse<DataPaged<Video>>
  {
    private readonly HttpService _httpService;
    private readonly ILogger<GetAllVideosService> _logger;

    public GetAllVideosService(HttpService httpService,
      ILogger<GetAllVideosService> logger)
    {
      _httpService = httpService;
      _logger = logger;
    }

    public override async Task<HttpResponse<DataPaged<Video>>> ExecuteAsync(GetAllVideosRequest request,
      CancellationToken cancellationToken = default)
    {
      var uri = string.Empty;
      if (string.IsNullOrEmpty(request.UserId))
      {
        uri = $"videos";
      }
      else
      {
        if (request.UserId.ToLower().Equals(ServiceConstants.ME))
        {
          uri = $"{request.UserId}/videos";
        }
        else
        {
          uri = $"users/{request.UserId}/videos";
        }
      }

      try
      {
        var query = new Dictionary<string, string>();

        query.AddIfNotNull("page", request.Page?.ToString());
        query.AddIfNotNull("per_page", request.PageSize?.ToString());

        var response = await _httpService.HttpGetAsync<DataPaged<Video>>($"{uri}", query);

        return response;
      }
      catch (Exception exception)
      {
        _logger.LogError(exception);
        return HttpResponse<DataPaged<Video>>.FromException(exception.Message);
      }
    }
  }
}
