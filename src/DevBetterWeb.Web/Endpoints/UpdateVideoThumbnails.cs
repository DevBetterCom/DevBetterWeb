using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Mvc;
using DevBetterWeb.Web.CustomAttributes;

namespace DevBetterWeb.Web.Endpoints;

[IntegrationApiAuthorization]
public class UpdateVideoThumbnails : EndpointBaseAsync
	.WithRequest<long>
	.WithResult<ActionResult<ArchiveVideoDto>>
{
	private readonly IRepository<ArchiveVideo> _repository;
	private readonly IMapper _mapper;
	private readonly CreateAnimatedThumbnailsService _createAnimatedThumbnailsService;
	private readonly GetAllAnimatedThumbnailService _getAllAnimatedThumbnailService;
	private readonly GetVideoService _getVideoService;

	public UpdateVideoThumbnails(IRepository<ArchiveVideo> repository, IMapper mapper, CreateAnimatedThumbnailsService createAnimatedThumbnailsService,
		GetAllAnimatedThumbnailService getAllAnimatedThumbnailService, GetVideoService getVideoService)
	{
		_repository = repository;
		_mapper = mapper;
		_createAnimatedThumbnailsService = createAnimatedThumbnailsService;
		_getAllAnimatedThumbnailService = getAllAnimatedThumbnailService;
		_getVideoService = getVideoService;
	}

	[HttpGet("videos/update-video-thumbnails/{videoId}")]
	public override async Task<ActionResult<ArchiveVideoDto>> HandleAsync(long videoId, CancellationToken cancellationToken = default)
	{
		var spec = new ArchiveVideoByVideoIdSpec(videoId.ToString());
		var existVideo = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
		if (existVideo == null)
		{
			return BadRequest();
		}

		var response = await _getVideoService.ExecuteAsync(videoId.ToString(), cancellationToken);
		if (response?.Data == null)
		{
			return BadRequest("Video Not Found!");
		}

		var existThumbsResponse = await _getAllAnimatedThumbnailService.ExecuteAsync(new GetAnimatedThumbnailRequest(videoId, null), cancellationToken);
		if (existThumbsResponse.Data.Total <= 0)
		{
			var getAnimatedThumbnailResult = await _createAnimatedThumbnailsService.ExecuteAsync(videoId, cancellationToken);
			if (getAnimatedThumbnailResult == null)
			{
				return BadRequest();
			}
			existVideo.AnimatedThumbnailUri = getAnimatedThumbnailResult.AnimatedThumbnailUri;
		}
		else
		{
			existVideo.AnimatedThumbnailUri = existThumbsResponse.Data.Data.FirstOrDefault()?.AnimatedThumbnailUri;
		}


		await _repository.UpdateAsync(existVideo, cancellationToken);
		var updatedVideo = _mapper.Map<ArchiveVideoDto>(existVideo);

		return Ok(updatedVideo);
	}
}
