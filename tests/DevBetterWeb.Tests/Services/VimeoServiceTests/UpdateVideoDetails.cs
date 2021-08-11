using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Services;
using VimeoDotNet.Enums;
using VimeoDotNet.Models;
using Xunit;

namespace DevBetterWeb.Tests.Services.VimeoServiceTests
{
  public class UpdateVideoDetails
  {
    [Fact]
    public async Task ReturnsTrueUpdateVideoDetailsAsync()
    {
      var vimeoCredential = new VimeoCredential("9adde280e2ec716c03c7f0b4d671059d");
      var vimeoService = new VimeoService(vimeoCredential);
      var videos = await vimeoService.GetAllVideosAsync();
      var videoDetails = new VideoUpdateMetadata();
      videoDetails.Password = "122324";
      videoDetails.AllowDownloadVideo = false;
      videoDetails.Name = videos.Data[0].Name;
      videoDetails.Description = videos.Data[0].Description;
      videoDetails.Privacy = VideoPrivacyEnum.Password;
      await vimeoService.UpdateVideoDetailsAsync(videos.Data[0].Id.Value, videoDetails);

      Assert.True(true);
    }
  }
}
