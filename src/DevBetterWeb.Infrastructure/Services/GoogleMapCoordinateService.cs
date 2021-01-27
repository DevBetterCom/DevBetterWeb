using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DevBetterWeb.Infrastructure.Services
{
  public class GoogleMapCoordinateService : IMapCoordinateService
  {
    private IConfiguration _configuration { get; }
    private IHttpClientFactory _factory { get; }

    public GoogleMapCoordinateService(IConfiguration configuration, IHttpClientFactory factory)
    {
      _configuration = configuration;
      _factory = factory;
    }

    public async Task<string> GetMapCoordinates(string address)
    {
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
        throw new Exception("Map API call failed");
      }
    }
  }
}


