using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace DevBetterWeb.Web.Pages.User
{
  public class MapModel : PageModel
  {
    private readonly AppDbContext _appDbContext;
    public List<MapCoordinates> AddressCoordinates { get; set; } = new List<MapCoordinates>();
    public IConfiguration _configuration { get; }

    public MapModel(AppDbContext appDbContext, IConfiguration configuration)
    {
      _appDbContext = appDbContext;
      _configuration = configuration;
    }
    public async Task OnGet()
    {
      var membersAddresses = (await _appDbContext.Members.ToListAsync()).Select(m => m.Address);
      
      foreach (var address in membersAddresses)
      {
        var fullAddressAPIResponse = JObject.Parse(await GetMapCoordinates(address));
        var latitude = fullAddressAPIResponse.SelectToken("results[0].geometry.location.lat").ToObject<decimal>();
        var longitude = fullAddressAPIResponse.SelectToken("results[0].geometry.location.lng").ToObject<decimal>();
        AddressCoordinates.Add(new MapCoordinates(latitude,longitude));
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
