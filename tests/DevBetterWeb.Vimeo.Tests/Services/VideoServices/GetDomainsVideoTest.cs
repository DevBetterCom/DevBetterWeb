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
  public class GetDomainsVideoServiceTest
  {
    private readonly GetDomainsVideoService _getDomainsVideoService;

    public GetDomainsVideoServiceTest()
    {
      var httpService = HttpServiceBuilder.Build();
      _getDomainsVideoService = GetDomainsVideoServiceBuilder.Build(httpService);
    }

    [Fact]
    public async Task ReturnsDomainsVideoTest()
    {
      var response = await _getDomainsVideoService
        .SetToken(AccountConstants.ACCESS_TOKEN)
        .ExecuteAsync("587044076");

      response.Data.ShouldNotBe(null);
    }

    private Stream GetFileFromEmbeddedResources(string relativePath)
    {
      var assembly = typeof(UploadVideoTest).GetTypeInfo().Assembly;
      return assembly.GetManifestResourceStream(relativePath);
    }
  }
}
