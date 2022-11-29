using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Web.Interfaces;
using Flurl.Http;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Web.Services;

public class VideoDetailsService : IVideoDetailsService
{
	private readonly GetVideoService _getVideoService;
	private readonly GetAllTextTracksService _getAllTextTracksService;
	private readonly IRepository<ArchiveVideo> _archiveVideoRepository;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IWebVTTParsingService _vttService;

	public VideoDetailsService(
		GetVideoService getVideoService,
		GetAllTextTracksService getAllTextTracksService,
		IRepository<ArchiveVideo> archiveVideoRepository,
		UserManager<ApplicationUser> userManager,
		IWebVTTParsingService vttService)
	{
		_getVideoService = getVideoService;
		_getAllTextTracksService = getAllTextTracksService;
		_archiveVideoRepository = archiveVideoRepository;
		_userManager = userManager;
		_vttService = vttService;
	}
	public async Task<(HttpResponse<Video>, string, ArchiveVideo?, ApplicationUser)> GetDataAsync(
		string videoId,
		string? currentUserName,
		string currentVideoURL)
	{
		var videoTask = _getVideoService.ExecuteAsync(videoId);
		var textTracksTask = _getAllTextTracksService.ExecuteAsync(videoId);
		var videoSpec = new ArchiveVideoByVideoIdFullAggregateSpec(videoId);
		var archiveVideoTask = _archiveVideoRepository.FirstOrDefaultAsync(videoSpec);
		var applicationUserTask = _userManager.FindByNameAsync(currentUserName!);

		var transcriptTask = GetTranscriptAsync((await textTracksTask).Data.Data, currentVideoURL);

		var task = Task.WhenAll(videoTask, transcriptTask, archiveVideoTask, applicationUserTask);
		try
		{
			await task;
		}
		catch (Exception)
		{
			if (task.Exception != null)
			{
				throw task.Exception;
			}

			throw;
		}

		return (videoTask!.Result, transcriptTask!.Result, archiveVideoTask!.Result, applicationUserTask!.Result);
	}

	public async Task IncrementViewsAndUpdate(ArchiveVideo archiveVideo)
	{
		archiveVideo.Views++;
		await _archiveVideoRepository.UpdateAsync(archiveVideo);
	}

	public async Task<string> GetTranscriptAsync(IEnumerable<TextTrack> textTracks, string videoURL)
	{
		try
		{
			if (textTracks.Any())
			{
				string textTrackLink = textTracks.First().Link;
				string vtt = await textTrackLink.GetStringAsync();
				return _vttService.Parse(vtt, videoURL, paragraphSize: 4);
			}

			return String.Empty;
		}
		catch (Exception)
		{
			return String.Empty;
		}
	}
}
