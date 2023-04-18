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
	private readonly IVideosService _videosService;

	public AddVideoInfo(IRepository<ArchiveVideo> repository, IMapper mapper, IVideosService videosService)
	{
		_repository = repository;
		_mapper = mapper;
		_videosService = videosService;
	}

	[HttpPost("videos/add-video-info")]
	public override async Task<ActionResult<ArchiveVideoDto>> HandleAsync([FromBody] ArchiveVideoDto request, CancellationToken cancellationToken = default)
	{
		var archiveVideo = _mapper.Map<ArchiveVideo>(request);

		await _videosService.AddArchiveVideoInfo(archiveVideo, true, cancellationToken);

		return Ok(request);
	}
}
