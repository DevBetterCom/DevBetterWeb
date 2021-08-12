﻿using System;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Vimeo.Models;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Vimeo.Extensions;
using Ardalis.ApiCaller;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class UserDetailsService : BaseAsyncApiCaller
    .WithRequest<string>
    .WithResponse<User>
  {
    private readonly HttpService _httpService;
    private readonly ILogger<UserDetailsService> _logger;

    public UserDetailsService(HttpService httpService, ILogger<UserDetailsService> logger)
    {
      _httpService = httpService;
      _logger = logger;
    }

    public override async Task<HttpResponse<User>> ExecuteAsync(string userId, CancellationToken cancellationToken = default)
    {
      var uri = $"/users/{userId}";
      try
      {
        var response = await _httpService.HttpGetAsync<User>(uri);

        return response;
      }
      catch (Exception exception)
      {
        _logger.LogError(exception);
        return HttpResponse<User>.FromException(exception.Message);
      }
    }
    public UserDetailsService SetToken(string token)
    {
      _httpService.SetAuthorization($"bearer {token}");

      return this;
    }
  }
}
