using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Mvc;
using DevBetterWeb.Web.CustomAttributes;

namespace DevBetterWeb.Web.Endpoints;

[UploaderApiAuthorization]
public class AddVideoInfo : EndpointBaseAsync
	.WithRequest<ArchiveVideoDto>
	.WithResult<ActionResult<ArchiveVideoDto>>
{
	private readonly IRepository<ArchiveVideo> _repository;
	private readonly IMapper _mapper;

	public AddVideoInfo(IRepository<ArchiveVideo> repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	[HttpPost("videos/add-video-info")]
	public override async Task<ActionResult<ArchiveVideoDto>> HandleAsync([FromBody] ArchiveVideoDto request, CancellationToken cancellationToken = default)
	{
		var archiveVideo = _mapper.Map<ArchiveVideo>(request);

		var spec = new ArchiveVideoByVideoIdSpec(archiveVideo.VideoId!);
		var existVideo = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
		if (existVideo == null)
		{
			var videoAddedEvent = new VideoAddedEvent(archiveVideo);
			archiveVideo.Events.Add(videoAddedEvent);
			_ = await _repository.AddAsync(archiveVideo, cancellationToken);
		}
		else
		{
			existVideo.Description = archiveVideo.Description;
			existVideo.Title = archiveVideo.Title;
			existVideo.Duration = archiveVideo.Duration;
			if (!string.IsNullOrEmpty(archiveVideo.AnimatedThumbnailUri))
			{
				existVideo.AnimatedThumbnailUri = archiveVideo.AnimatedThumbnailUri;
			}

			await _repository.UpdateAsync(existVideo, cancellationToken);
		}

		return Ok(request);
	}
}
