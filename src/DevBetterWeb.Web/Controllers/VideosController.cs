using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Pages.Admin.Videos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Web.Controllers;

[Route("videos")]
[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
[ApiController]
public class VideosController : Controller
{
  private readonly string _expectedApiKey;
  private readonly IMapper _mapper;
  private readonly GetOEmbedVideoService _getOEmbedVideoService;
  private readonly GetVideoService _getVideoService;
  private readonly DeleteVideoService _deleteVideoService;
  private readonly UploadSubtitleToVideoService _uploadSubtitleToVideoService;
  private readonly IRepository<ArchiveVideo> _repository;
  private readonly IMarkdownService _markdownService;

  public VideosController(IMapper mapper,
    IRepository<ArchiveVideo> repository,
    IOptions<ApiSettings> apiSettings,
    GetOEmbedVideoService getOEmbedVideoService,
    GetVideoService getVideoService,
    DeleteVideoService deleteVideoService,
    UploadSubtitleToVideoService uploadSubtitleToVideoService,
    IMarkdownService markdownService)
  {
    _mapper = mapper;
    _getOEmbedVideoService = getOEmbedVideoService;
    _getVideoService = getVideoService;
    _deleteVideoService = deleteVideoService;
    _uploadSubtitleToVideoService = uploadSubtitleToVideoService;
    _repository = repository;
    _expectedApiKey = apiSettings.Value.ApiKey;
    _markdownService = markdownService;
  }

  [HttpPost("list")]
  public async Task<IActionResult> ListAsync([FromForm] DataTableParameterModel dataTableParameterModel)
  {
    var draw = dataTableParameterModel.Draw;
    var length = dataTableParameterModel.Length;
    int pageSize = length != null ? Convert.ToInt32(length) : 20;
    var startIndex = Convert.ToInt32(dataTableParameterModel.Start);

    var filterSpec = new ArchiveVideoFilteredSpec(dataTableParameterModel.Search);
    var totalRecords = await _repository.CountAsync(filterSpec);

    var pagedSpec = new ArchiveVideoByPageSpec(startIndex, pageSize, dataTableParameterModel.Search);
    var archiveVideos = await _repository.ListAsync(pagedSpec);
    var archiveVideosDto = _mapper.Map<List<ArchiveVideoDto>>(archiveVideos);

    var jsonData = new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = archiveVideosDto };

    return Ok(jsonData);
  }

  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
  [HttpPost("update-description")]
  public async Task<IActionResult> UpdateDescriptionAsync([FromForm] UpdateDescription updateDescription)
  {
    var video = await _getVideoService.ExecuteAsync(updateDescription.VideoId);
    if (video?.Data == null) return NotFound($"Video Not Found {updateDescription.VideoId}");

    var oEmbed = await _getOEmbedVideoService.ExecuteAsync(video.Data.Link);
    if (oEmbed?.Data == null) return NotFound($"Video Not Found {updateDescription.VideoId}");

    var spec = new ArchiveVideoByVideoIdSpec(updateDescription.VideoId);
    var archiveVideo = await _repository.GetBySpecAsync(spec);
    if (archiveVideo == null)
    {
      return NotFound($"Video Not Found {updateDescription.VideoId}");
    }

    archiveVideo.Description = updateDescription.Description;
    await _repository.UpdateAsync(archiveVideo);
    await _repository.SaveChangesAsync();

    var oEmbedViewModel = new OEmbedViewModel(oEmbed.Data);
    oEmbedViewModel.VideoId = int.Parse(archiveVideo.VideoId);
    oEmbedViewModel.DescriptionMd = _markdownService.RenderHTMLFromMD(archiveVideo.Description);
    oEmbedViewModel.Description = archiveVideo.Description;
    oEmbedViewModel
      .BuildHtml(video?.Data?.Link);

    return Ok(oEmbedViewModel);
  }

  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
  [HttpPost("upload-subtitle")]
  public async Task<IActionResult> UploadSubtitleAsync([FromForm] UploadSubtitleRequest request)
  {
    var uploadSubtitleToVideoRequest = new UploadSubtitleToVideoRequest(request.VideoId, request.Subtitle, request.Language);
    var response = await _uploadSubtitleToVideoService.ExecuteAsync(uploadSubtitleToVideoRequest);
    if (!response.Data || response.Code != System.Net.HttpStatusCode.OK) return NotFound($"Subtitle Not Found {request.VideoId}");

    return Ok();
  }


  [AllowAnonymous]
  [HttpPost("add-video-info")]
  public async Task<IActionResult> AddVideoInfoAsync([FromBody] ArchiveVideoDto archiveVideoDto)
  {
    var apiKey = Request.Headers[Constants.ConfigKeys.ApiKey];

    if (apiKey != _expectedApiKey)
    {
      return Unauthorized();
    }
    if (archiveVideoDto == null)
    {
      return BadRequest();
    }

    var archiveVideo = _mapper.Map<ArchiveVideo>(archiveVideoDto);

    var spec = new ArchiveVideoByVideoIdSpec(archiveVideo.VideoId);
    var existVideo = await _repository.GetBySpecAsync(spec);
    if (existVideo == null)
    {
      archiveVideo = await _repository.AddAsync(archiveVideo);
    }
    else
    {
      existVideo.Description = archiveVideo.Description;
      existVideo.Title = archiveVideo.Title;
      existVideo.Duration = archiveVideo.Duration;
      existVideo.AnimatedThumbnailUri = archiveVideo.AnimatedThumbnailUri;
      await _repository.UpdateAsync(existVideo);
    }

    return Ok(archiveVideo);
  }

  [AllowAnonymous]
  [HttpPost("update-video-thumbnails")]
  public async Task<IActionResult> UpdateVideoThumbnailsAsync([FromBody] ArchiveVideoDto archiveVideoDto)
  {
    var apiKey = Request.Headers[Constants.ConfigKeys.ApiKey];

    if (apiKey != _expectedApiKey)
    {
      return Unauthorized();
    }

    var archiveVideo = _mapper.Map<ArchiveVideo>(archiveVideoDto);

    var spec = new ArchiveVideoByVideoIdSpec(archiveVideo.VideoId);
    var existVideo = await _repository.GetBySpecAsync(spec);
    if (existVideo == null)
    {
      return BadRequest();
    }
    else
    {
      existVideo.AnimatedThumbnailUri = archiveVideo.AnimatedThumbnailUri;
      await _repository.UpdateAsync(existVideo);
    }

    return Ok(archiveVideo);
  }

  [AllowAnonymous]
  [HttpDelete("delete-video")]
  public async Task<IActionResult> DeleteVideoThAsync([FromQuery] string vimeoVideoId)
  {
    var apiKey = Request.Headers[Constants.ConfigKeys.ApiKey];

    if (apiKey != _expectedApiKey)
    {
      return Unauthorized();
    }

    await _deleteVideoService.ExecuteAsync(vimeoVideoId);

    var spec = new ArchiveVideoByVideoIdSpec(vimeoVideoId);
    var existVideo = await _repository.GetBySpecAsync(spec);
    if (existVideo == null)
    {
      return BadRequest();
    }

    await _repository.DeleteAsync(existVideo);

    return Ok();
  }
}
