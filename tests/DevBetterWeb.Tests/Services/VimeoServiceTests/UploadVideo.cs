using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Services;
using Xunit;

namespace DevBetterWeb.Tests.Services.VimeoServiceTests
{
  public class UploadVideo
  {
    [Fact]
    public async Task ReturnsUploadVideoAsync()
    {
      var vimeoCredential = new VimeoCredential("PVT");
      var vimeoService = new VimeoService(vimeoCredential);

      var stream = GetFileFromEmbeddedResources("DevBetterWeb.Tests." + "2020-10-23 MyHouseApp Status Call.mp4");
      Assert.NotNull(stream);

      var buffer = new byte[stream.Length];
      await stream.ReadAsync(buffer, 0, (int)stream.Length);
      var isSuccess = await vimeoService.UploadVideoAsync("2020-10-23 MyHouseApp Status Call", buffer);

      Assert.True(isSuccess);
    }

    private Stream GetFileFromEmbeddedResources(string relativePath)
    {
      var assembly = typeof(UploadVideo).GetTypeInfo().Assembly;
      return assembly.GetManifestResourceStream(relativePath);
    }
  }
}
