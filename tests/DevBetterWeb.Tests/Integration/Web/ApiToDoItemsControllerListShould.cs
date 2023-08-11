using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Web;
using Newtonsoft.Json;
using Xunit;

namespace DevBetterWeb.Tests.Integration.Web;

public class ApiToDoItemsControllerList : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client;

  public ApiToDoItemsControllerList(CustomWebApplicationFactory<Program> factory)
  {
    _client = factory.CreateClient();
  }

  //[Fact]
  //public async Task ReturnsTwoItems()
  //{
  //    var response = await _client.GetAsync("/api/todoitems");
  //    response.EnsureSuccessStatusCode();
  //    var stringResponse = await response.Content.ReadAsStringAsync();
  //    var result = JsonConvert.DeserializeObject<IEnumerable<ToDoItem>>(stringResponse).ToList();

  //    Assert.Equal(2, result.Count());
  //    Assert.Equal(1, result.Count(a => a.Title == "Test Item 1"));
  //    Assert.Equal(1, result.Count(a => a.Title == "Test Item 2"));
  //}
}
