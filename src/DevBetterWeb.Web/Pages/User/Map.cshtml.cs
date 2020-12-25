using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Drawing;
using Markdig.Extensions.Tables;

namespace DevBetterWeb.Web.Pages.User
{
  public class MapModel : PageModel
  {
    private readonly AppDbContext _appDbContext;
    public List<MapCoordinate> AddressCoordinates { get; set; } = new List<MapCoordinate>();

    public MapModel(AppDbContext appDbContext)
    {
      _appDbContext = appDbContext;
    }
    public async Task OnGet()
    {
      var membersAddresses = (await _appDbContext.Members.ToListAsync()).Select(m => m.Address);
      
      foreach (var address in membersAddresses)
      {
        var fullAddressAPIResponse = JObject.Parse(await GetMapCoordinates(address));
        var latitude = fullAddressAPIResponse.SelectToken("results[0].geometry.location.lat").ToObject<decimal>();
        var longitude = fullAddressAPIResponse.SelectToken("results[0].geometry.location.lng").ToObject<decimal>();
        AddressCoordinates.Add(new MapCoordinate(latitude,longitude));
      }
    }

    private async Task<string> GetMapCoordinates(string address)
    {
      var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key=AIzaSyCm2QJJErqEhoJClMjWUSEpNh5KwnSjD1A";

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

    public class MapCoordinate
    {
      public decimal Latitude { get; set; }

      public decimal Longitude { get; set; }

      public MapCoordinate(decimal latitude, decimal longitude)
      {
        Latitude = latitude;
        Longitude = longitude;
      }
    }

    
  }
}
