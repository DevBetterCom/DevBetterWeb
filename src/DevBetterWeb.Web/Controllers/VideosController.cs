using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
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
	private readonly IRepository<ArchiveVideo> _repository;
	private readonly IMarkdownService _markdownService;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IRepository<Member> _memberRepository;

	public VideosController(IMapper mapper,
		IRepository<ArchiveVideo> repository,
		IOptions<ApiSettings> apiSettings,
		GetOEmbedVideoService getOEmbedVideoService,
		GetVideoService getVideoService,
		IMarkdownService markdownService,
		UserManager<ApplicationUser> userManager,
		IRepository<Member> memberRepository)
	{
		_mapper = mapper;
		_getOEmbedVideoService = getOEmbedVideoService;
		_getVideoService = getVideoService;
		_repository = repository;
		_expectedApiKey = apiSettings.Value.ApiKey;
		_markdownService = markdownService;
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

		var currentUserName = User.Identity!.Name;
		var applicationUser = await _userManager.FindByNameAsync(currentUserName);

		var memberSpec = new MemberByUserIdWithFavoriteArchiveVideosSpec(applicationUser.Id);
		var member = await _memberRepository.FirstOrDefaultAsync(memberSpec);

		if (member is null)
		{
			return Unauthorized();
		}

		var pagedSpec = new ArchiveVideoByPageSpec(startIndex, pageSize, dataTableParameterModel.Search, dataTableParameterModel.FilterFavorites, member.Id);
		var totalRecords = await _repository.CountAsync(pagedSpec);
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
			Status = av.Status,
			Title = av.Title,
			VideoId = av.VideoId,
			VideoUrl = av.VideoUrl,
			Views = av.Views
		});

		var currentPage = Math.Ceiling((decimal)(startIndex + 1) / pageSize);
		var jsonData = new { draw = draw, currentPage = currentPage, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = archiveVideosDto };

		return Ok(jsonData);
	}

	[HttpGet("inprogress/{videoId}")]
	public async Task<IActionResult> VideoInProgressAsync(string videoId)
	{
		var userId = _userManager.GetUserId(User);
		var memberByUserSpec = new MemberByUserIdSpec(userId);
		var member = await _memberRepository.FirstOrDefaultAsync(memberByUserSpec);
		if (member == null)
		{
			return Unauthorized();
		}

		var spec = new ArchiveVideoByVideoIdWithProgressSpec(videoId);
		var existVideo = await _repository.FirstOrDefaultAsync(spec);
		if (existVideo == null)
		{
			return NotFound("Video not found!");
		}

		var progress = existVideo.MembersVideoProgress.FirstOrDefault(x => x.ArchiveVideoId == existVideo.Id && x.MemberId == member.Id);
		if (progress == null)
		{
			var progressToAdd = new MemberVideoProgress(member.Id, existVideo.Id, existVideo.Duration);
			progressToAdd.SetToInProgress();
			existVideo.AddVideoProgress(progressToAdd);
		}
		else
		{
			progress.SetToInProgress();
		}

		await _repository.UpdateAsync(existVideo);

		return Ok(Json(new { success = true, responseText = "Done!" }));
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

		return Ok(Json(new { success = true, responseText = "Your message successfully sent!" }));
	}

	[HttpPut("favorite-video/{vimeoVideoId}")]
	public async Task<IActionResult> PutToggleFavoriteVideo([FromRoute] string vimeoVideoId)
	{
		var currentUserName = User.Identity!.Name;
		var applicationUser = await _userManager.FindByNameAsync(currentUserName);

		var memberSpec = new MemberByUserIdWithFavoriteArchiveVideosSpec(applicationUser.Id);
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
