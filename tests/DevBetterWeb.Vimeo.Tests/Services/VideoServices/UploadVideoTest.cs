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
using DevBetterWeb.Vimeo.Tests.Constants;
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
      var loggerGetStreamingTicketService = new Mock<ILogger<GetStreamingTicketService>>().Object;

      var loggerUploadVideoService = new Mock<ILogger<UploadVideoService>>().Object;
      var loggerCompleteUploadByCompleteUriService = new Mock<ILogger<CompleteUploadByCompleteUriService>>().Object;
      var loggerUpdateVideoDetailsService = new Mock<ILogger<UpdateVideoDetailsService>>().Object;

      var httpClient = new HttpClient { BaseAddress = new Uri(ServiceConstants.VIMEO_URI) };
      var httpService = new HttpService(httpClient);

      var getStreamingTicketService = new GetStreamingTicketService(httpService, loggerGetStreamingTicketService);
      var completeUploadService = new CompleteUploadByCompleteUriService(httpService, loggerCompleteUploadByCompleteUriService);
      var updateVideoDetailsService = new UpdateVideoDetailsService(httpService, loggerUpdateVideoDetailsService);

      _uploadVideoService = new UploadVideoService(httpService, loggerUploadVideoService, getStreamingTicketService, completeUploadService, updateVideoDetailsService);
    }

    [Fact]
    public async Task ReturnsAccountDetailsTest()
    {
      var request = new UploadVideoRequest();
      request.UserId = "me";
      request.Video.Name = "Test";
      request.Video.Description = "Test";
      request.Video.Privacy.View = PrivacyViewType.PASSWORD_TYPE;
      request.Video.Privacy.Password = AccessType.PRIVATE_TYPE;
      request.Video.Privacy.Embed = AccessType.PUBLIC_TYPE;
      request.Video.Password = "122324";
      request.Video.Privacy.Download = false;

      var stream = GetFileFromEmbeddedResources("DevBetterWeb.Vimeo.Tests." + "2020-10-23 MyHouseApp Status Call.mp4");
      Assert.NotNull(stream);

      var buffer = new byte[stream.Length];
      await stream.ReadAsync(buffer, 0, (int)stream.Length);

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
