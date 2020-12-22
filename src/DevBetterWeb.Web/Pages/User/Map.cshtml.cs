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

namespace DevBetterWeb.Web.Pages.User
{
  public class MapModel : PageModel
  {
    private readonly AppDbContext _appDbContext;

    public List<string> AddressCoordinates { get; set; } = new List<string>();

    public MapModel(AppDbContext appDbContext)
    {
      _appDbContext = appDbContext;
    }
    public async Task OnGet()
    {
      var members = await _appDbContext.Members.ToListAsync();
      var memberAddresses = members.Select(m => m.Address);
      
      foreach (var address in memberAddresses)
      {
        var response = JObject.Parse(await GetMapCoordinates(address));
        
        AddressCoordinates.Add(response.SelectToken("results[0].geometry.location").ToString());


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
          return null;
        }
      }
    }
  }
}
