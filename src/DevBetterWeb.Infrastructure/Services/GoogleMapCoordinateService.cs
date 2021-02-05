using System;
using System.Net.Http;
using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Infrastructure.Services
{
  public class GoogleMapCoordinateService : IMapCoordinateService
  {
    private readonly ILogger<GoogleMapCoordinateService> _logger;

    private IConfiguration _configuration { get; }
    private IHttpClientFactory _factory { get; }

    public GoogleMapCoordinateService(IConfiguration configuration,
      IHttpClientFactory factory,
      ILogger<GoogleMapCoordinateService> logger)
    {
      _configuration = configuration;
      _factory = factory;
      _logger = logger;
    }

    public async Task<string> GetMapCoordinates(string address)
    {
      _logger.LogInformation($"Getting map info for address {address}");

      var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key={_configuration["GoogleMapsAPIKey"]}";

      var client = _factory.CreateClient();
      client.BaseAddress = new Uri(url);

      var response = await client.GetAsync(url);

      if (response.IsSuccessStatusCode)
      {
        var result = await response.Content.ReadAsStringAsync();
        return result;
      }
      else
      {
        _logger.LogError($"Error getting map info: HTTP Status: {response.StatusCode}");
        throw new Exception("Map API call failed");
      }
    }
  }
}


