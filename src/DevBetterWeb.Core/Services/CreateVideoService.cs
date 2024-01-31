using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using NimblePros.Vimeo.Interfaces;
using NimblePros.Vimeo.Models;
using NimblePros.Vimeo.VideoServices;
using NimblePros.Vimeo.VideoTusService;

namespace DevBetterWeb.Core.Services;
public class CreateVideoService : ICreateVideoService
{
	private readonly GetVideoService _getVideoService;
	private readonly IUploadVideoTusService _uploadVideoTusService;
	private readonly IRepository<ArchiveVideo> _repositoryArchiveVideo;
	private readonly IAddCreatedVideoToFolderService _addCreatedVideoToFolderService;

	public CreateVideoService(GetVideoService getVideoService, IUploadVideoTusService uploadVideoTusService, IRepository<ArchiveVideo> repositoryArchiveVideo, IAddCreatedVideoToFolderService addCreatedVideoToFolderService)
	{
		_getVideoService = getVideoService;
		_uploadVideoTusService = uploadVideoTusService;
		_repositoryArchiveVideo = repositoryArchiveVideo;
		_addCreatedVideoToFolderService = addCreatedVideoToFolderService;
	}

	public async Task<string> StartAsync(string videoName, long videoSize, string domain, CancellationToken cancellationToken = default)
	{
		var uploadVideoRequest = new UploadVideoRequest(UploadApproach.Tus)
		{
			Name = videoName,
			Upload = { Size = videoSize },
			Embed = new Embed { Title = new Title { Owner = EmbedOwnerTitle.Hide }, Speed = true, Volume = true, Playbar = true, Buttons = new Buttons { Embed = true, Hd = true, Fullscreen = true } },
			Privacy = new Privacy { Embed = EmbedPrivacy.Whitelist, View = PrivacyView.Disable, Download = false },
			EmbedDomains = new List<string> { domain },
			HideFromVimeo = true
		};
		var sessionId = await _uploadVideoTusService.StartAsync(uploadVideoRequest, cancellationToken);

		return sessionId;
	}

	public async Task<UploadChunkStatus> UploadChunkAsync(bool isBaseFolder, string sessionId, string chunk, string? description, long? folderId, CancellationToken cancellationToken = default)
	{
		var result = await _uploadVideoTusService.UploadChunkAsync(sessionId, Convert.FromBase64String(chunk), cancellationToken);
		if (result.UploadChunkStatus == UploadChunkStatus.Completed)
		{
			var addArchive = await AddArchiveVideoAsync(result.VideoId, description, cancellationToken);
			if (addArchive == null)
			{
				return UploadChunkStatus.Error;
			}

			_ = await _addCreatedVideoToFolderService.ExecuteAsync(isBaseFolder, folderId, addArchive, cancellationToken);
		}

		return result.UploadChunkStatus;
	}

	private async Task<ArchiveVideo?> AddArchiveVideoAsync(long videoId, string? description, CancellationToken cancellationToken = default)
	{
		if (videoId <= 0)
		{
			return null;
		}
		var response = await _getVideoService.ExecuteAsync(videoId, cancellationToken);
		if (!response.IsSuccess)
		{
			return null;
		}
		var archiveVideo = new ArchiveVideo
		{
			Title = response.Data.Name,
			DateCreated = response.Data.CreatedTime,
			DateUploaded = DateTimeOffset.UtcNow,
			Duration = response.Data.Duration * 1000,
			VideoId = response.Data.Id.ToString(),
			VideoUrl = response.Data.Uri,
			Description = description
		};

		var spec = new ArchiveVideoByVideoIdSpec(archiveVideo.VideoId);
		var existVideo = await _repositoryArchiveVideo.FirstOrDefaultAsync(spec, cancellationToken);
		if (existVideo == null)
		{
			var videoAddedEvent = new VideoAddedEvent(archiveVideo);
			archiveVideo.Events.Add(videoAddedEvent);

			_ = await _repositoryArchiveVideo.AddAsync(archiveVideo, cancellationToken);

			return archiveVideo;
		}
		existVideo.Description = archiveVideo.Description;
		existVideo.Title = archiveVideo.Title;
		existVideo.Duration = archiveVideo.Duration;

		await _repositoryArchiveVideo.UpdateAsync(existVideo, cancellationToken);

		return existVideo;
	}
}
