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
using Microsoft.Extensions.DependencyInjection;

namespace DevBetterWeb.Web.Endpoints;

[UploaderApiOrRoleAuthorization(AuthConstants.Roles.ADMINISTRATORS)]
public class UploadVideo : EndpointBaseAsync
	.WithRequest<UploadVideoResumableInfo>
	.WithResult<ActionResult<UploadVideoResumableInfo>>
{
	private readonly IBackgroundTaskQueue _backgroundTaskQueue;
	private readonly UploadResumableVideoService _uploadResumableVideoService;
	private readonly IServiceScopeFactory _serviceScopeFactory;

	public UploadVideo(
		IBackgroundTaskQueue backgroundTaskQueue,
		UploadResumableVideoService uploadResumableVideoService,
		IServiceScopeFactory serviceScopeFactory)
	{
		_backgroundTaskQueue = backgroundTaskQueue;
		_uploadResumableVideoService = uploadResumableVideoService;
		_serviceScopeFactory = serviceScopeFactory;
	}

	[HttpPost("videos/upload-video")]
	public override async Task<ActionResult<UploadVideoResumableInfo>> HandleAsync([FromBody] UploadVideoResumableInfo request, CancellationToken cancellationToken = default)
	{
		var result = await _uploadResumableVideoService.ExecuteAsync(request, cancellationToken);

		if (result.Data.FileFullSize == result.Data.UploadOffset)
		{
			var allowedDomain = Request.GetUri().Authority;
			_backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
			{
				using var scope = _serviceScopeFactory.CreateScope();
				var updateVideoDetailsService = scope.ServiceProvider.GetRequiredService<UpdateVideoDetailsService>();
				var addDomainToVideoService = scope.ServiceProvider.GetRequiredService<AddDomainToVideoService>();
				var getVideoService = scope.ServiceProvider.GetRequiredService<GetVideoService>();
				var videosService = scope.ServiceProvider.GetRequiredService<IVideosService>();

				await AddVimeoVideoInfoAsync(request, updateVideoDetailsService, addDomainToVideoService, allowedDomain, token);
				await AddArchiveVideoInfoAsync(request, getVideoService, videosService, token);
			});
		}

		return Ok(result?.Data);
	}

	private async Task AddVimeoVideoInfoAsync(UploadVideoResumableInfo request, UpdateVideoDetailsService updateVideoDetailsService, AddDomainToVideoService addDomainToVideoService, string allowedDomain, CancellationToken cancellationToken = default)
	{
		var video = new Video();
		video
			.SetVideoUrl(request.VideoUrl)
			.SetName(Path.GetFileNameWithoutExtension(request.FileName))
			.SetEmbedProtecedPrivacy()
			.SetEmbed();

		var updateVideoDetailsRequest = new UpdateVideoDetailsRequest(long.Parse(video.Id), video);
		_ = await updateVideoDetailsService.ExecuteAsync(updateVideoDetailsRequest, cancellationToken);

		var addDomainRequest = new AddDomainToVideoRequest(long.Parse(video.Id), allowedDomain);
		_ = await addDomainToVideoService.ExecuteAsync(addDomainRequest, cancellationToken);
	}

	private async Task AddArchiveVideoInfoAsync(UploadVideoResumableInfo request, GetVideoService getVideoService, IVideosService videosService, CancellationToken cancellationToken = default)
	{
		var response = await getVideoService.ExecuteAsync(request.VideoId, cancellationToken);
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

		await videosService.AddArchiveVideoInfo(archiveVideo, false, cancellationToken);
	}
}
