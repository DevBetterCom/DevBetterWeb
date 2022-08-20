using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Extensions;
using DevBetterWeb.Vimeo.Models;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class GetPagedVideosService : BaseAsyncApiCaller
  .WithRequest<GetAllVideosRequest>
  .WithResponse<DataPaged<Video>>
{
  private readonly HttpService _httpService;
  private readonly ILogger<GetPagedVideosService> _logger;

  public GetPagedVideosService(HttpService httpService,
    ILogger<GetPagedVideosService> logger)
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
	    uri = request.UserId.ToLower().Equals(ServiceConstants.ME) ? $"{request.UserId}/videos" : $"users/{request.UserId}/videos";
    }

    try
    {
      var query = new Dictionary<string, string>();

      query.AddIfNotNull("page", request.Page?.ToString());
      query.AddIfNotNull("per_page", request.PageSize?.ToString());

			var response = await _httpService.HttpGetAsync<DataPaged<Video>>($"{uri}", query, cancellationToken);

      return response;
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
      return HttpResponse<DataPaged<Video>>.FromException(exception.Message);
    }
  }
}
