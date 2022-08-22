﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Models;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class GetVideoService : BaseAsyncApiCaller
  .WithRequest<string>
  .WithResponse<Video>
{
  private readonly HttpService _httpService;
  private readonly ILogger<GetVideoService> _logger;

  public GetVideoService(HttpService httpService, ILogger<GetVideoService> logger)
  {
    _httpService = httpService;
    _logger = logger;
  }

  public override async Task<HttpResponse<Video>> ExecuteAsync(string videoId, CancellationToken cancellationToken = default)
  {
    var uri = "videos";
		var fullPath = $"{uri}/{videoId}";

		try
    {
      var response = await _httpService.HttpGetAsync<Video>(fullPath, cancellationToken);

      return response;
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, $"Error with URL: {0} ", fullPath);
      return HttpResponse<Video>.FromException(exception.Message);
    }
  }
}
