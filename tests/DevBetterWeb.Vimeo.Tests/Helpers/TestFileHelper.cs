using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Vimeo.Tests.Constants;

namespace DevBetterWeb.Vimeo.Tests.Helpers
{
  public class TestFileHelper
  {
    private readonly UploadVideoService _uploadVideoService;
    private readonly DeleteVideoService _deleteVideoService;

    public TestFileHelper()
    {
      var httpService = HttpServiceBuilder.Build();
      _uploadVideoService = UploadVideoServiceBuilder.Build(httpService);
      _deleteVideoService = DeleteVideoServiceBuilder.Build(httpService);
    }

    public async Task<long> UploadTest()
    {
      var buffer = await BuildAsync();

      var video = new Video();
      video.Name = "Test";
      video.Description = "Test";
      video.Privacy.View = PrivacyViewType.DISABLE_TYPE;
      video.Privacy.Embed = AccessType.WHITELIST_TYPE;
      video.Privacy.Download = false;

      var request = new UploadVideoRequest("me", buffer, video);

      request.FileData = buffer;

      var response = await _uploadVideoService
        .ExecuteAsync(request);

      return response.Data;
    }

    public async Task<HttpStatusCode> DeleteTestFile(string id)
    {
      var response = await _deleteVideoService
        .ExecuteAsync(id);

      return response.Code;      
    }

    public static async Task<byte[]> BuildAsync()
    {
      var stream = GetFileFromEmbeddedResources("DevBetterWeb.Vimeo.Tests." + "Test.mp4");

      var buffer = new byte[stream.Length];
      await stream.ReadAsync(buffer, 0, (int)stream.Length);

      return buffer;
    }

    private static Stream GetFileFromEmbeddedResources(string relativePath)
    {
      var assembly = typeof(UploadVideoTest).GetTypeInfo().Assembly;
      return assembly.GetManifestResourceStream(relativePath);
    }
  }
}
