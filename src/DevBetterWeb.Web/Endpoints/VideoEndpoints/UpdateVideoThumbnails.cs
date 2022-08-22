using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Mvc;
using DevBetterWeb.Web.CustomAttributes;

namespace DevBetterWeb.Web.Endpoints;

[UploaderApiAuthorization]
public class UpdateVideoThumbnails : EndpointBaseAsync
	.WithRequest<long>
	.WithResult<ActionResult<ArchiveVideoDto>>
{
	private readonly IMapper _mapper;
	private readonly IVideosService _videosService;

	public UpdateVideoThumbnails(IMapper mapper, IVideosService videosService)
	{
		_mapper = mapper;
		_videosService = videosService;
	}

	[HttpGet("videos/update-video-thumbnails/{videoId}")]
	public override async Task<ActionResult<ArchiveVideoDto>> HandleAsync(long videoId, CancellationToken cancellationToken = default)
	{
		var response = await _videosService.UpdateVideoThumbnailsAsync(videoId, cancellationToken);
		if (response == null)
		{
			return NotFound();
		}

		var updatedVideo = _mapper.Map<ArchiveVideoDto>(response);
		return Ok(updatedVideo);
	}
}
