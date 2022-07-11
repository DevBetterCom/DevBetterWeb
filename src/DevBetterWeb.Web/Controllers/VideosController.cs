using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Pages.Admin.Videos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
  private readonly CreateAnimatedThumbnailsService _createAnimatedThumbnailsService;
  private readonly GetAllAnimatedThumbnailService _getAllAnimatedThumbnailService;
  private readonly IVideosService _videosService;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IRepository<Member> _memberRepository;

  public VideosController(IMapper mapper,
    IRepository<ArchiveVideo> repository,
    IOptions<ApiSettings> apiSettings,
    GetOEmbedVideoService getOEmbedVideoService,
    GetVideoService getVideoService,
    DeleteVideoService deleteVideoService,
    UploadSubtitleToVideoService uploadSubtitleToVideoService,
    IMarkdownService markdownService,
    CreateAnimatedThumbnailsService createAnimatedThumbnailsService,
    GetAllAnimatedThumbnailService getAllAnimatedThumbnailService,
    IVideosService videosService,
    UserManager<ApplicationUser> userManager,
    IRepository<Member> memberRepository)
  {
    _mapper = mapper;
    _getOEmbedVideoService = getOEmbedVideoService;
    _getVideoService = getVideoService;
    _deleteVideoService = deleteVideoService;
    _uploadSubtitleToVideoService = uploadSubtitleToVideoService;
    _repository = repository;
    _expectedApiKey = apiSettings.Value.ApiKey;
    _markdownService = markdownService;
    _createAnimatedThumbnailsService = createAnimatedThumbnailsService;
    _getAllAnimatedThumbnailService = getAllAnimatedThumbnailService;
    _videosService = videosService;
    _userManager = userManager;
    _memberRepository = memberRepository;
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

	var currentUserName = User.Identity!.Name;
    var applicationUser = await _userManager.FindByNameAsync(currentUserName);

    var memberSpec = new  MemberByUserIdWithFavoriteArchiveVideosSpec(applicationUser.Id);
    var member = await _memberRepository.FirstOrDefaultAsync(memberSpec);

    if (member is null)
    {
      return Unauthorized();
    }

    var pagedSpec = new ArchiveVideoByPageSpec(startIndex, pageSize, dataTableParameterModel.Search, dataTableParameterModel.FilterFavorites, member.Id);
    var archiveVideos = await _repository.ListAsync(pagedSpec);

    var archiveVideosDto = archiveVideos.Select(av => new ArchiveVideoDto
	{
		AnimatedThumbnailUri = av.AnimatedThumbnailUri,
		DateCreated = av.DateCreated,
		DateUploaded = av.DateUploaded,
		Description = av.Description,
		Duration = av.Duration,
		IsMemberFavorite = member.FavoriteArchiveVideos.Any(fav => fav.ArchiveVideoId == av.Id),
		MemberFavoritesCount = av.MemberFavorites.Count(),
		ShowNotes = av.ShowNotes,
		Status = av.Status,
		Title = av.Title,
		VideoId = av.VideoId,
		VideoUrl = av.VideoUrl,
		Views = av.Views
	});

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

    var spec = new ArchiveVideoByVideoIdSpec(updateDescription.VideoId!);
    var archiveVideo = await _repository.FirstOrDefaultAsync(spec);
    if (archiveVideo == null)
    {
      return NotFound($"Video Not Found {updateDescription.VideoId}");
    }

    archiveVideo.Description = updateDescription.Description;
    await _repository.UpdateAsync(archiveVideo);
    await _repository.SaveChangesAsync();

    var oEmbedViewModel = new OEmbedViewModel(oEmbed.Data);
    oEmbedViewModel.VideoId = int.Parse(archiveVideo.VideoId!);
    oEmbedViewModel.DescriptionMd = _markdownService.RenderHTMLFromMD(archiveVideo.Description);
    oEmbedViewModel.Description = archiveVideo.Description;
    oEmbedViewModel
      .BuildHtml(video.Data.Link);

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

  [HttpPost("submit-comment-reply")]
  public async Task<IActionResult> SubmitCommentReply([FromForm] CommentReplyRequest request)
  {
	var userId = _userManager.GetUserId(User);
	var memberByUserSpec = new MemberByUserIdSpec(userId);
    var member = await _memberRepository.FirstOrDefaultAsync(memberByUserSpec);
    if (member == null)
    {
	  return Unauthorized();
    }

	var spec = new ArchiveVideoByVideoIdSpec(request.VideoId!);
	var existVideo = await _repository.FirstOrDefaultAsync(spec);
	if (existVideo == null)
	{
		return NotFound("Video not found!");
	}
	existVideo.AddComment(new VideoComment(member.Id, existVideo.Id, request.CommentReplyToSubmit));
	await _repository.UpdateAsync(existVideo);

	return Json(new { success = true, responseText = "Your message successfuly sent!" });
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

    var archiveVideo = _mapper.Map<ArchiveVideo>(archiveVideoDto);

    var spec = new ArchiveVideoByVideoIdSpec(archiveVideo.VideoId!);
    var existVideo = await _repository.FirstOrDefaultAsync(spec);
    if (existVideo == null)
    {
      archiveVideo = await _repository.AddAsync(archiveVideo);
      var videoAddedEvent = new VideoAddedEvent(archiveVideo);
      archiveVideo.Events.Add(videoAddedEvent);
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
      
      await _repository.UpdateAsync(existVideo);
    }

    return Ok(archiveVideo);
  }

  [AllowAnonymous]
  [HttpGet("update-video-thumbnails/{videoId}")]
  public async Task<IActionResult> UpdateVideoThumbnailsAsync(long videoId)
  {
    var apiKey = Request.Headers[Constants.ConfigKeys.ApiKey];

    if (apiKey != _expectedApiKey)
    {
      return Unauthorized();
    }

    var spec = new ArchiveVideoByVideoIdSpec(videoId.ToString());
    var existVideo = await _repository.FirstOrDefaultAsync(spec);
    if (existVideo == null)
    {
      return BadRequest();
    }

    var response = await _getVideoService.ExecuteAsync(videoId.ToString());
    if (response?.Data == null)
    {
      return BadRequest("Video Not Found!");
    }

    var existThumbsResponse = await _getAllAnimatedThumbnailService.ExecuteAsync(new GetAnimatedThumbnailRequest(videoId, null));
    if (existThumbsResponse.Data.Total <= 0)
    {
      var getAnimatedThumbnailResult = await _createAnimatedThumbnailsService.ExecuteAsync(videoId);
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


    await _repository.UpdateAsync(existVideo);

    return Ok(existVideo);
  }

  [AllowAnonymous]
  [HttpGet("update-all-videos-thumbnails")]
  public async Task<IActionResult> UpdateAllVideosThumbnailsAsync()
  {
    var apiKey = Request.Headers[Constants.ConfigKeys.ApiKey];

    if (apiKey != _expectedApiKey)
    {
      return Unauthorized();
    }

    await _videosService.UpdateVideosThumbnail(null);
    
    return Ok();
  }

  [AllowAnonymous]
  [HttpGet("delete-all-videos-no-vimeo")]
  public async Task<IActionResult> DeleteAllVideosNoVimeoAsync()
  {
    var apiKey = Request.Headers[Constants.ConfigKeys.ApiKey];

    if (apiKey != _expectedApiKey)
    {
      return Unauthorized();
    }

    await _videosService.DeleteVideosNotExistOnVimeoFromVimeo(null);
    await _videosService.DeleteVideosNotExistOnVimeoFromDatabase(null);

    return Ok();
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

    var spec = new ArchiveVideoByVideoIdSpec(archiveVideo.VideoId!);
    var existVideo = await _repository.FirstOrDefaultAsync(spec);
    if (existVideo == null)
    {
      return BadRequest();
    }

    existVideo.AnimatedThumbnailUri = archiveVideo.AnimatedThumbnailUri;
    await _repository.UpdateAsync(existVideo);

    return Ok(archiveVideo);
  }

  [AllowAnonymous]
  [HttpDelete("uploader/delete-video/{vimeoVideoId}")]
  public async Task<IActionResult> DeleteVideoThAsync([FromRoute] string vimeoVideoId)
  {
    var apiKey = Request.Headers[Constants.ConfigKeys.ApiKey];

    if (apiKey != _expectedApiKey)
    {
      return Unauthorized();
    }

    var spec = new ArchiveVideoByVideoIdSpec(vimeoVideoId);
    var existVideo = await _repository.FirstOrDefaultAsync(spec);
    if (existVideo != null)
    {
      await _repository.DeleteAsync(existVideo);
    }

    await _deleteVideoService.ExecuteAsync(vimeoVideoId);

    return Ok();
  }

  [HttpPut("favorite-video/{vimeoVideoId}")]
  public async Task<IActionResult> PutToggleFavoriteVideo([FromRoute] string vimeoVideoId)
  {
    var currentUserName = User.Identity!.Name;
    var applicationUser = await _userManager.FindByNameAsync(currentUserName);

    var memberSpec = new  MemberByUserIdWithFavoriteArchiveVideosSpec(applicationUser.Id);
    var member = await _memberRepository.FirstOrDefaultAsync(memberSpec);

    if (member is null)
    {
      return Unauthorized();
    }

    var videoSpec = new ArchiveVideoByVideoIdSpec(vimeoVideoId);
    var archiveVideo = await _repository.FirstOrDefaultAsync(videoSpec);
    if (archiveVideo == null) return NotFound($"Video Not Found {vimeoVideoId}");

    if (member.FavoriteArchiveVideos.Any(v => v.ArchiveVideoId == archiveVideo.Id))
    {
      member.RemoveFavoriteArchiveVideo(archiveVideo);
    }
    else
    {
      member.AddFavoriteArchiveVideo(archiveVideo);
    }

    await _memberRepository.UpdateAsync(member);

	return Ok(new { archiveVideo.VideoId });
  }
}
