using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Builders;
using DevBetterWeb.Vimeo.Tests.Constants;
using DevBetterWeb.Vimeo.Tests.Helpers;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Vimeo.Tests
{
  public class GetOEmbedVideoTest
  {
    private readonly GetOEmbedVideoService _getOEmbedVideoService;

    public GetOEmbedVideoTest()
    {
      var httpService = HttpServiceBuilder.Build();
      _getOEmbedVideoService = GetOEmbedVideoServiceBuilder.Build(httpService);
    }

    [Fact]
    public async Task ReturnsOEmbedVideoTest()
    {
      var response = await _getOEmbedVideoService
        .SetToken(AccountConstants.ACCESS_TOKEN)
        .ExecuteAsync("https://vimeo.com/videos/585883066");

      response.Data.ShouldNotBe(null);
    }

    private Stream GetFileFromEmbeddedResources(string relativePath)
    {
      var assembly = typeof(UploadVideoTest).GetTypeInfo().Assembly;
      return assembly.GetManifestResourceStream(relativePath);
    }
  }
}
