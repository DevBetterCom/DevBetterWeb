﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Logging;
using NimblePros.Vimeo.Models;
using NimblePros.Vimeo.Services.VideoServices;
using NimblePros.Vimeo.VideoServices;

namespace DevBetterWeb.Infrastructure.Services;

public class VideosCacheService: IVideosCacheService
{
	public bool IsEmpty => _vimeoVideos.Count <= 0;
	private readonly ILogger<VideosCacheService> _logger;
	private readonly GetVideosUserAppearsService _getAllVideosService;
	private readonly List<Video> _vimeoVideos = new List<Video>();

	public VideosCacheService(ILogger<VideosCacheService> logger, GetVideosUserAppearsService getAllVideosService)
	{
		_logger = logger;
		_getAllVideosService = getAllVideosService;
	}

	public async Task UpdateAllVideosAsync()
	{
		_logger.LogInformation("Videos update started");

		var request = new GetVideosUserAppearsRequest();
		var videosResponse = await _getAllVideosService.ExecuteAsync(request);
		if (videosResponse?.Data == null)
		{
			_logger.LogInformation("No Videos on vimeo server");
			return;
		}
		_logger.LogInformation($"Videos from vimeo server: {videosResponse.Data.Data.Count}");

		Clear();
		AddRange(videosResponse.Data.Data);

		_logger.LogInformation("Videos update finished");
	}

	public List<Video> GetAllVideos()
	{
		return _vimeoVideos;
	}

	private void Add(Video video)
	{
		_vimeoVideos.Add(video);
	}

	private void AddRange(List<Video> videos)
	{
		_vimeoVideos.AddRange(videos);
	}

	private void Remove(Video video)
	{
		_vimeoVideos.Remove(video);
	}

	private void Clear()
	{
		_vimeoVideos.Clear();
	}
}
