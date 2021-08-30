﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Vimeo.Services.VideoServices;

namespace DevBetterWeb.UploaderApp
{
  public class UploaderManager
  {
    private static string MP4_FILES = "*.mp4";
    private static string MD_FILES = "*.md";

    private readonly HttpService _httpService;
    private readonly UploadVideoService _uploadVideoService;
    private readonly GetAllVideosService _getAllVideosService;

    public UploaderManager(string token)
    {
      _httpService = Builders.BuildHttpService(token);
       _uploadVideoService = Builders.BuildUploadVideoService(_httpService);
      _getAllVideosService = Builders.BuildGetAllVideosService(_httpService);
    }

    public async Task SyncAsync(string folderToUpload)
    {
      var videos = GetVideos(folderToUpload);            

      var allExisVideos = await GetExistVideosAsync();
      foreach (var video in videos)
      {
        if (allExisVideos.Any(x => x.Name.ToLower() == video.Name.ToLower()))
        {
          Console.WriteLine($"{video.Name} already exists - skipping.");
          continue;
        }
        Console.WriteLine($"Starting Uploading {video.Name}");
        if (string.IsNullOrEmpty(video.Description))
        {
          Console.WriteLine($"{video.Name} has no associated MD file(s)...");
        }
        var request = new UploadVideoRequest(ServiceConstants.ME, video.Data, video);
        request.FileData = video.Data;

        var response = await _uploadVideoService.ExecuteAsync(request);
        if (response.Data > 0)
        {
          Console.WriteLine($"{video.Name} Uploaded!");
        }
        else
        {
          Console.WriteLine($"{video.Name} Upload Error!");
        }
      }
    }

    private async Task<List<Video>> GetExistVideosAsync()
    {      
      var getAllVideosRequest = new GetAllVideosRequest(ServiceConstants.ME);
      var allExisVideos = await _getAllVideosService.ExecuteAsync(getAllVideosRequest);

      return allExisVideos.Data.Data;
    }

    private List<Video> GetVideos(string folderPath)
    {
      var result = new List<Video>();
      if (!Directory.Exists(folderPath))
      {
        return result;
      }

      string[] videosPaths = Directory.GetFiles(folderPath, MP4_FILES, SearchOption.AllDirectories);
      if (videosPaths == null)
      {
        return result;
      }

      string[] mdsPaths = Directory.GetFiles(folderPath, MD_FILES, SearchOption.AllDirectories);
      if (mdsPaths == null)
      {
        return result;
      }

      foreach (var videoPath in videosPaths)
      {
        var video = new Video();
        video.SetName(Path.GetFileNameWithoutExtension(videoPath));

        var mdFilePath = mdsPaths.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.ToLower().Trim()) == Path.GetFileNameWithoutExtension(videoPath.ToLower().Trim()));
        var description = string.IsNullOrEmpty(mdFilePath) ? string.Empty : File.ReadAllText(mdFilePath);
        video.SetDescription(description);

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
