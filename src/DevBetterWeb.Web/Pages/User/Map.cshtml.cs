using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace DevBetterWeb.Web.Pages.User
{
  public class MapModel : PageModel
  {
    public List<MapCoordinates> AddressCoordinates { get; set; } = new List<MapCoordinates>();
    public IRepository _repository { get; }
    public IConfiguration _configuration { get; }

    public MapModel(IRepository repository, IConfiguration configuration)
    {
      _repository = repository;
      _configuration = configuration;
    }
    public async Task OnGet()
    {
      var membersAddresses = (await _repository.ListAsync<Member>()).Select(m => m.Address);
      
      foreach (var address in membersAddresses)
      {
        var addressParts = address.Split(',');
        var cityDetailsAPIResponse = JObject.Parse(await GetMapCoordinates(addressParts[1]));
        var latResponse = cityDetailsAPIResponse.SelectToken("results[0].geometry.location.lat");
        var lngResponse = cityDetailsAPIResponse.SelectToken("results[0].geometry.location.lng");

        if (latResponse != null && lngResponse != null)
        {
          var latitude = latResponse.ToObject<decimal>();
          var longitude = lngResponse.ToObject<decimal>();
          AddressCoordinates.Add(new MapCoordinates(latitude, longitude));
        }
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
