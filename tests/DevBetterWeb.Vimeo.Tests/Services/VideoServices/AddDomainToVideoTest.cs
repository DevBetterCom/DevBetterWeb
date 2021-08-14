using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Builders;
using DevBetterWeb.Vimeo.Tests.Constants;
using DevBetterWeb.Vimeo.Tests.Helpers;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Vimeo.Tests
{
  public class AddDomainToVideoServiceTest
  {
    private readonly AddDomainToVideoService _addDomainToVideoService;

    public AddDomainToVideoServiceTest()
    {
      var httpService = HttpServiceBuilder.Build();
      _addDomainToVideoService = AddDomainToVideoServiceBuilder.Build(httpService);
    }

    [Fact]
    public async Task ReturnsAddDomainToVideoTest()
    {
      var response = await _addDomainToVideoService
        .SetToken(AccountConstants.ACCESS_TOKEN)
        .ExecuteAsync(new AddDomainToVideoRequest(587044076, "localhost:5010"));

      response.Data.ShouldNotBe(false);
    }
  }
}
