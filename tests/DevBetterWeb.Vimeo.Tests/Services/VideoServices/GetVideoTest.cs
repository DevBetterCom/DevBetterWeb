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
  public class GetVideoTest
  {
    private readonly GetVideoService _getVideoService;

    public GetVideoTest()
    {
      var httpService = HttpServiceBuilder.Build();
      _getVideoService = GetVideoServiceBuilder.Build(httpService);
    }

    [Fact]
    public async Task ReturnsAccountDetailsTest()
    {
      var response = await _getVideoService
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
