using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.AspNetCore.Mvc;
using DevBetterWeb.Web.CustomAttributes;
using DevBetterWeb.Vimeo.Models;
using System.IO;
using System;
using System.Net;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using DevBetterWeb.Core;

namespace DevBetterWeb.Web.Endpoints;

[UploaderApiOrRoleAuthorization(AuthConstants.Roles.ADMINISTRATORS)]
public class UploadVideo : EndpointBaseAsync
	.WithRequest<UploadVideoResumableInfo>
	.WithResult<ActionResult<UploadVideoResumableInfo>>
{
	private readonly UploadResumableVideoService _uploadResumableVideoService;
	private readonly UpdateVideoDetailsService _updateVideoDetailsService;
	private readonly AddDomainToVideoService _addDomainToVideoService;
	private readonly GetVideoService _getVideoService;
	private readonly IVideosService _videosService;

	public UploadVideo(
		UploadResumableVideoService uploadResumableVideoService, 
		UpdateVideoDetailsService updateVideoDetailsService, 
		AddDomainToVideoService addDomainToVideoService,
		GetVideoService getVideoService,
		IVideosService videosService)
	{
		_uploadResumableVideoService = uploadResumableVideoService;
		_updateVideoDetailsService = updateVideoDetailsService;
		_addDomainToVideoService = addDomainToVideoService;
		_getVideoService = getVideoService;
		_videosService = videosService;
	}

	[HttpPost("videos/upload-video")]
	public override async Task<ActionResult<UploadVideoResumableInfo>> HandleAsync([FromBody] UploadVideoResumableInfo request, CancellationToken cancellationToken = default)
	{
		var result = await _uploadResumableVideoService.ExecuteAsync(request, cancellationToken);

		if (result.Data.FileFullSize == result.Data.UploadOffset)
		{
			await AddVimeoVideoInfoAsync(request, cancellationToken);
			await AddArchiveVideoInfoAsync(request, cancellationToken);
		}

		return Ok(result?.Data);
	}

	private async Task AddVimeoVideoInfoAsync(UploadVideoResumableInfo request, CancellationToken cancellationToken = default)
	{
		var video = new Video();
		video
			.SetVideoUrl(request.VideoUrl)
			.SetName(Path.GetFileNameWithoutExtension(request.FileName))
			.SetEmbedProtecedPrivacy()
			.SetEmbed();

		var updateVideoDetailsRequest = new UpdateVideoDetailsRequest(long.Parse(video.Id), video);
		_ = await _updateVideoDetailsService.ExecuteAsync(updateVideoDetailsRequest, cancellationToken);

		var allowedDomain = Request.GetUri().Authority;
		var addDomainRequest = new AddDomainToVideoRequest(long.Parse(video.Id), allowedDomain);
		_ = await _addDomainToVideoService.ExecuteAsync(addDomainRequest, cancellationToken);
	}

	private async Task AddArchiveVideoInfoAsync(UploadVideoResumableInfo request, CancellationToken cancellationToken = default)
	{
		var response = await _getVideoService.ExecuteAsync(request.VideoId, cancellationToken);
		if (response.Code != HttpStatusCode.OK)
		{
			return;
		}
		var archiveVideo = new ArchiveVideo
		{
			Title = request.VideoName,
			DateCreated = request.CreatedTime,
			DateUploaded = DateTimeOffset.UtcNow,
			Duration = response.Data.Duration*1000,
			VideoId = request.VideoId,
			VideoUrl = request.VideoUrl
		};

		await _videosService.AddArchiveVideoInfo(archiveVideo, cancellationToken);
	}
}
