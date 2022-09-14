using System;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Web.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Web.Services;

public class VideoDetailsService : IVideoDetailsService
{
	private readonly GetVideoService _getVideoService;
	private readonly GetAllTextTracksService _getAllTextTracksService;
	private readonly IRepository<ArchiveVideo> _archiveVideoRepository;
	private readonly UserManager<ApplicationUser> _userManager;

	public VideoDetailsService(
		GetVideoService getVideoService,
		GetAllTextTracksService getAllTextTracksService,
		IRepository<ArchiveVideo> archiveVideoRepository,
		UserManager<ApplicationUser> userManager)
	{
		_getVideoService = getVideoService;
		_getAllTextTracksService = getAllTextTracksService;
		_archiveVideoRepository = archiveVideoRepository;
		_userManager = userManager;
	}
	public async Task<(HttpResponse<Video>, HttpResponse<GetAllTextTracksResponse>, ArchiveVideo?, ApplicationUser)> GetDataAsync(string videoId, string? currentUserName)
	{
		var videoTask = _getVideoService.ExecuteAsync(videoId);
		var textTracksTask = _getAllTextTracksService.ExecuteAsync(videoId);
		var videoSpec = new ArchiveVideoByVideoIdFullAggregateSpec(videoId);
		var archiveVideoTask = _archiveVideoRepository.FirstOrDefaultAsync(videoSpec);
		var applicationUserTask = _userManager.FindByNameAsync(currentUserName);

		var task = Task.WhenAll(videoTask, textTracksTask, archiveVideoTask, applicationUserTask);
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

		return (videoTask.Result, textTracksTask.Result, archiveVideoTask.Result, applicationUserTask.Result);
	}

	public async Task IncrementViewsAndUpdate(ArchiveVideo archiveVideo)
	{
		archiveVideo.Views++;
		await _archiveVideoRepository.UpdateAsync(archiveVideo);
	}
}