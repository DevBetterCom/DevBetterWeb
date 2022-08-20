using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Mvc;
using DevBetterWeb.Web.CustomAttributes;

namespace DevBetterWeb.Web.Endpoints;

[UploaderApiAuthorization]
public class GetAllVideos : EndpointBaseAsync
	.WithoutRequest
	.WithResult<ActionResult<List<ArchiveVideoDto>>>
{
	private readonly IRepository<ArchiveVideo> _repository;
	private readonly IMapper _mapper;
	private readonly IVideosCacheService _videosCacheService;

	public GetAllVideos(IRepository<ArchiveVideo> repository, IMapper mapper, IVideosCacheService videosCacheService)
	{
		_repository = repository;
		_mapper = mapper;
		_videosCacheService = videosCacheService;
	}

	[HttpGet("videos/get-all-videos")]
	public override async Task<ActionResult<List<ArchiveVideoDto>>> HandleAsync(CancellationToken cancellationToken = default)
	{
		var archiveVideos = await _repository.ListAsync(cancellationToken);
		var videosDto = _mapper.Map<List<ArchiveVideoDto>>(archiveVideos);
		var vimeoVideos = _videosCacheService.GetAllVideos();
		foreach (var videoDto in videosDto)
		{
			videoDto.IsInfoUploaded = true;

			var vimeoVideo = vimeoVideos.FirstOrDefault(x => x.Name.ToLower() == videoDto.Title?.ToLower());
			if (vimeoVideo == null)
			{
				videoDto.IsUploaded = false;
			}
			else
			{
				videoDto.IsUploaded = true;
				videoDto.DateCreated = vimeoVideo.CreatedTime;
				videoDto.Title = vimeoVideo.Name;
				videoDto.Duration = vimeoVideo.Duration;
			}
		}

		foreach (var vimeoVideo in vimeoVideos)
		{
			var videoDto = videosDto.FirstOrDefault(x => x.Title?.ToLower() == vimeoVideo.Name?.ToLower());
			if (videoDto == null)
			{
				var videosDtoToAdd = new ArchiveVideoDto();
				videosDtoToAdd.Title = vimeoVideo.Name;
				videosDtoToAdd.VideoId = vimeoVideo.Id;
				videosDtoToAdd.VideoUrl = vimeoVideo.Uri;
				videosDtoToAdd.DateUploaded = vimeoVideo.CreatedTime;
				videosDtoToAdd.IsUploaded = true;
				videosDtoToAdd.IsInfoUploaded = false;
				videosDtoToAdd.Duration = vimeoVideo.Duration;

				videosDto.Insert(0, videosDtoToAdd);
			}
		}

		return Ok(videosDto);
	}
}
