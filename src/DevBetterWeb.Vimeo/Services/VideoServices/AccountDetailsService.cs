using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Models;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Vimeo.Extensions;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class AccountDetailsService : BaseAsyncApiCaller
    .WithoutRequest
    .WithResponse<User>
  {
    private readonly HttpService _httpService;
    private readonly ILogger<Upload> _logger;
    private readonly UserDetailsService _userDetailsService;

    public AccountDetailsService(HttpService httpService, ILogger<Upload> logger, UserDetailsService userDetailsService)
    {
      _httpService = httpService;
      _logger = logger;
      _userDetailsService = userDetailsService;
    }

    public override async Task<HttpResponse<User>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
      try
      {
        var response = await _userDetailsService.ExecuteAsync("me");

        return response;
      }
      catch (Exception exception)
      {
        _logger.LogError(exception);
        return HttpResponse<User>.FromException(exception.Message);
      }
    }

    public AccountDetailsService SetToken(string token)
    {
      _httpService.SetAuthorization(token);

      return this;
    }
  }
}
