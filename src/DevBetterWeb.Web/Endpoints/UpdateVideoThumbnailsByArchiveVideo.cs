using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Mvc;
using DevBetterWeb.Web.CustomAttributes;

namespace DevBetterWeb.Web.Endpoints;

[IntegrationApiAuthorization]
public class UpdateVideoThumbnailsByArchiveVideo : EndpointBaseAsync
	.WithRequest<ArchiveVideoDto>
	.WithResult<ActionResult<ArchiveVideoDto>>
{
	private readonly IRepository<ArchiveVideo> _repository;
	private readonly IMapper _mapper;

	public UpdateVideoThumbnailsByArchiveVideo(IRepository<ArchiveVideo> repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	[HttpPost("videos/update-video-thumbnails")]
	public override async Task<ActionResult<ArchiveVideoDto>> HandleAsync([FromBody] ArchiveVideoDto archiveVideoDto, CancellationToken cancellationToken = default)
	{
		var archiveVideo = _mapper.Map<ArchiveVideo>(archiveVideoDto);

		var spec = new ArchiveVideoByVideoIdSpec(archiveVideo.VideoId!);
		var existVideo = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
		if (existVideo == null)
		{
			return BadRequest();
		}

		existVideo.AnimatedThumbnailUri = archiveVideo.AnimatedThumbnailUri;
		await _repository.UpdateAsync(existVideo, cancellationToken);

		var updatedVideo = _mapper.Map<ArchiveVideoDto>(existVideo);
		return Ok(updatedVideo);
	}
}
