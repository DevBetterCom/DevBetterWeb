using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Vimeo.Services.UserServices;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Builders;
using DevBetterWeb.Vimeo.Tests.Constants;
using DevBetterWeb.Vimeo.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Vimeo.Tests
{
  public class UploadVideoTest
  {
    private readonly UploadVideoService _uploadVideoService;

    public UploadVideoTest()
    {
      var httpService = HttpServiceBuilder.Build();
      _uploadVideoService = UploadVideoServiceBuilder.Build(httpService);
    }

    [Fact]
    public async Task ReturnsAccountDetailsTest()
    {
      var stream = GetFileFromEmbeddedResources("DevBetterWeb.Vimeo.Tests." + "2020-10-23 MyHouseApp Status Call.mp4");
      Assert.NotNull(stream);

      var buffer = new byte[stream.Length];
      await stream.ReadAsync(buffer, 0, (int)stream.Length);

      var video = new Video();
      video.Name = "Test";
      video.Description = "Test";
      video.Privacy.View = PrivacyViewType.DISABLE_TYPE;
      video.Privacy.Embed = AccessType.WHITELIST_TYPE;
      video.Privacy.Download = false;

      var request = new UploadVideoRequest("me", buffer, video);

      request.FileData = buffer;

      var response = await _uploadVideoService
        .SetToken(AccountConstants.ACCESS_TOKEN)
        .ExecuteAsync(request);

      response.Data.ShouldNotBe(0);
    }

    private Stream GetFileFromEmbeddedResources(string relativePath)
    {
      var assembly = typeof(UploadVideoTest).GetTypeInfo().Assembly;
      return assembly.GetManifestResourceStream(relativePath);
    }
  }
}
