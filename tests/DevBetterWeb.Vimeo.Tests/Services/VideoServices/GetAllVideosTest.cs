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
  public class GetAllVideosTest
  {
    private readonly GetAllVideosService _getAllVideosService;

    public GetAllVideosTest()
    {
      var httpService = HttpServiceBuilder.Build();
      _getAllVideosService = GetAllVideosServiceBuilder.Build(httpService);
    }

    [Fact]
    public async Task ReturnsAccountDetailsTest()
    {
      var request = new GetAllVideosRequest("me");
      var response = await _getAllVideosService
        .SetToken(AccountConstants.ACCESS_TOKEN)
        .ExecuteAsync(request);

      response.Data.ShouldNotBe(null);
    }

    private Stream GetFileFromEmbeddedResources(string relativePath)
    {
      var assembly = typeof(UploadVideoTest).GetTypeInfo().Assembly;
      return assembly.GetManifestResourceStream(relativePath);
    }
  }
}
