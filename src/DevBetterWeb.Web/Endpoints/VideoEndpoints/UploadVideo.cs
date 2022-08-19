using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.AspNetCore.Mvc;
using DevBetterWeb.Web.CustomAttributes;
using DevBetterWeb.Vimeo.Models;
using System.IO;
using DevBetterWeb.Vimeo.Constants;
using System;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;

namespace DevBetterWeb.Web.Endpoints;

[UploaderApiAuthorization]
public class UploadVideo : EndpointBaseAsync
	.WithRequest<UploadVideoResumableInfo>
	.WithResult<ActionResult<UploadVideoResumableInfo>>
{
	private readonly UploadResumableVideoService _uploadResumableVideoService;
	private readonly UpdateVideoDetailsService _updateVideoDetailsService;
	private readonly AddDomainToVideoService _addDomainToVideoService;
	private readonly CompleteUploadService _completeUploadService;

	public UploadVideo(
		UploadResumableVideoService uploadResumableVideoService, 
		UpdateVideoDetailsService updateVideoDetailsService, 
		AddDomainToVideoService addDomainToVideoService,
		CompleteUploadService completeUploadService)
	{
		_uploadResumableVideoService = uploadResumableVideoService;
		_updateVideoDetailsService = updateVideoDetailsService;
		_addDomainToVideoService = addDomainToVideoService;
		_completeUploadService = completeUploadService;
	}

	[HttpPost("videos/upload-video")]
	public override async Task<ActionResult<UploadVideoResumableInfo>> HandleAsync([FromBody] UploadVideoResumableInfo request, CancellationToken cancellationToken = default)
	{
		var result = await _uploadResumableVideoService.ExecuteAsync(request, cancellationToken);

		if (result.Data.FileFullSize == result.Data.UploadOffset)
		{
			await AddVimeoVideoInfoAndGetVideoIdAsync(request, cancellationToken);
		}

		return Ok(result?.Data);
	}

	private async Task AddVimeoVideoInfoAndGetVideoIdAsync(UploadVideoResumableInfo request, CancellationToken cancellationToken = default)
	{
		var completeUploadRequest = new CompleteUploadRequest(request.UploadUrl);
		var completeUploadResponse = await _completeUploadService.ExecuteAsync(completeUploadRequest, cancellationToken);

		var video = new Video();
		video
			.SetVideoUrl(request.VideoUrl)
			.SetName(Path.GetFileNameWithoutExtension(request.FileName))
			.SetEmbedProtecedPrivacy()
			.SetEmbed();

		var updateVideoDetailsRequest = new UpdateVideoDetailsRequest(long.Parse(video.Id), video);
		var updateVideoDetailsResponse = await _updateVideoDetailsService.ExecuteAsync(updateVideoDetailsRequest, cancellationToken);

		var allowedDomain = Request.GetUri().Authority;
		var addDomainRequest = new AddDomainToVideoRequest(long.Parse(video.Id), allowedDomain);
		var addDomainToVideoResponse = await _addDomainToVideoService.ExecuteAsync(addDomainRequest, cancellationToken);
	}
}
