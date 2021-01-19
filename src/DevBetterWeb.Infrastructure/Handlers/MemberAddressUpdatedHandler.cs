using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace DevBetterWeb.Infrastructure.Handlers
{
  class MemberAddressUpdatedHandler : IHandle<MemberAddressUpdatedEvent>
  {
    public MemberAddressUpdatedHandler(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public IConfiguration _configuration { get; }

    public async Task Handle(MemberAddressUpdatedEvent addressUpdatedEvent)
    {
      var member = addressUpdatedEvent.Member;
      var addressParts = member.Address?.Split(',');

      if (addressParts is not null)
      {
        var cityDetailsAPIResponse = JObject.Parse(await GetMapCoordinates(addressParts[1]));
        var latResponse = cityDetailsAPIResponse.SelectToken("results[0].geometry.location.lat");
        var lngResponse = cityDetailsAPIResponse.SelectToken("results[0].geometry.location.lng");

        member.CityLatitude = latResponse?.ToObject<decimal>();
        member.CityLongitude = lngResponse?.ToObject<decimal>();
      }
    }

    private async Task<string> GetMapCoordinates(string address)
    {
      var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key={_configuration["GoogleMapsAPIKey"]}";

      using (var client = new HttpClient())
      {
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
}


