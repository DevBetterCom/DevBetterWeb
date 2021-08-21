using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Vimeo.Services.VideoServices;

namespace DevBetterWeb.UploaderApp
{
  class Program
  {
    static async Task Main(string[] args)
    {
      var argsList = args.ToList();
      if (argsList.Count == 0 || argsList.All( x => x.ToLower() != "-d") || argsList.All(x => x.ToLower() != "-t"))
      {
        Console.WriteLine("PLease use -d [destination folder] -t [Vimeo token]");
        return;
      }

      var folderToUploadIndex = argsList.FindIndex(x => x.ToLower() == "-d") + 1;
      if(folderToUploadIndex <= 0)
      {
        Console.WriteLine("PLease use -d [destination folder]");
        return;
      }

      var tokenIndex = argsList.FindIndex(x => x.ToLower() == "-t") + 1;
      if (tokenIndex <= 0)
      {
        Console.WriteLine("PLease use -t [Vimeo token]");
        return;
      }

      var folderToUpload = argsList[folderToUploadIndex];
      var videos = GetVideos(folderToUpload);

      var token = argsList[tokenIndex];
      var httpService = Builders.BuildHttpService(token);
      var uploader = Builders.BuildUploadVideoService(httpService);

      var allExisVideos = await GetExistVideosAsync(httpService);
      foreach (var video in videos)
      {
        if (allExisVideos.Any(x => x.Name.ToLower() == video.Name.ToLower())) {
          Console.WriteLine($"{video.Name} Exist");
          continue;
        }
        Console.WriteLine($"Starting Uploading {video.Name}");
        var request = new UploadVideoRequest("me", video.Data, video);
        request.FileData = video.Data;

        var response = await uploader.ExecuteAsync(request);
        if (response.Data > 0)
        {
          Console.WriteLine($"{video.Name} Uploaded");
        }else
        {
          Console.WriteLine($"{video.Name} Upload Error");
        }        
      }

      Console.WriteLine("Done, press any key to close");
      Console.ReadKey();
    }    

    private static async Task<List<Video>> GetExistVideosAsync(HttpService httpService)
    {
      var allExistVideosService = Builders.BuildGetAllVideosService(httpService);

      var getAllVideosRequest = new GetAllVideosRequest("me");
      var allExisVideos = await allExistVideosService.ExecuteAsync(getAllVideosRequest);

      return allExisVideos.Data.Data;
    }

    private static List<Video> GetVideos(string folderPath)
    {
      var result = new List<Video>();
      if (!Directory.Exists(folderPath))
      {
        return result;
      }

      string[] videosPaths = Directory.GetFiles(folderPath, "*.mp4", SearchOption.AllDirectories);
      if (videosPaths == null)
      {
        return result;
      }

      string[] mdsPaths = Directory.GetFiles(folderPath, "*.md", SearchOption.AllDirectories);
      if (mdsPaths == null)
      {
        return result;
      }

      foreach (var videoPath in videosPaths)
      {
        var video = new Video();
        video.SetName(Path.GetFileNameWithoutExtension(videoPath));

        var mdFilePath = mdsPaths.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.ToLower().Trim()) == Path.GetFileNameWithoutExtension(videoPath.ToLower().Trim()));
        video.SetDescription(File.ReadAllText(mdFilePath));

        video.Data = File.ReadAllBytes(videoPath);
        if (video.Data == null || video.Data.Length <= 0)
        {
          continue;
        }
        video.SetEmbedProtecedPrivacy();
        result.Add(video);
      }

      return result;
    }
  }
}
