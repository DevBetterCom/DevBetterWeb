using System;
using System.Net;
using Ardalis.ApiCaller;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Vimeo.Extensions;

namespace DevBetterWeb.Vimeo.Services.UserServices
{
  public class LoginService : BaseApiCaller
    .WithRequest<string>
    .WithResponse<bool>
  {
    private readonly HttpService _httpService;
    private readonly ILogger<LoginService> _logger;

    public LoginService(HttpService httpService, ILogger<LoginService> logger)
    {
      _httpService = httpService;
      _logger = logger;
    }

    public override HttpResponse<bool> Execute(string token)
    { 
      try
      {
        _httpService.SetAuthorization($"bearer {token}");

        return HttpResponse<bool>.FromHttpResponseMessage(true, HttpStatusCode.OK);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception);
        return HttpResponse<bool>.FromException(exception.Message);
      }
    }
  }
}
