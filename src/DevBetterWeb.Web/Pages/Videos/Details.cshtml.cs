using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Pages.Admin.Videos;
using DevBetterWeb.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Videos;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
public class DetailsModel : PageModel
{
	[BindProperty]
	public OEmbedViewModel? OEmbedViewModel { get; set; }
	public string? Transcript { get; set; }

	private readonly GetOEmbedVideoService _getOEmbedVideoService;
	private readonly GetVideoService _getVideoService;
	private readonly IRepository<ArchiveVideo> _repository;
	private readonly IMapper _mapper;
	private readonly IMarkdownService _markdownService;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IRepository<Member> _memberRepository;
	private readonly GetAllTextTracksService _getAllTextTracksService;
	private readonly HttpClient _httpClient;
	private readonly WebVTTParsingService _vttService;

	public DetailsModel(IMapper mapper, IMarkdownService markdownService, GetOEmbedVideoService getOEmbedVideoService,
		GetVideoService getVideoService, IRepository<ArchiveVideo> repository, UserManager<ApplicationUser> userManager,
		IRepository<Member> memberRepository, GetAllTextTracksService getAllTextTracksService, HttpClient httpClient, WebVTTParsingService vttService)
	{
		_mapper = mapper;
		_markdownService = markdownService;
		_getVideoService = getVideoService;
		_repository = repository;
		_getOEmbedVideoService = getOEmbedVideoService;
		_userManager = userManager;
		_memberRepository = memberRepository;
		_getAllTextTracksService = getAllTextTracksService;
		_httpClient = httpClient;
		_vttService = vttService;
	}

	public async Task<IActionResult> OnGet(string videoId, string? startTime = null)
	{
		var (video, textTracks, archiveVideo, applicationUser) = await GetDataAsync(videoId);
		if (video?.Data == null) return NotFound($"Video Not Found {videoId}");
		if (archiveVideo == null) return NotFound($"Video Not Found {videoId}");

		var (oEmbed, member) = await GetMoreDataAsync(video.Data.Link, applicationUser.Id);
		if (oEmbed?.Data == null) return NotFound($"Video Not Found {videoId}");
		if (member == null) return NotFound($"Member Not Found {applicationUser.Id}");

		if (textTracks?.Data != null)
		{
			await GetTranscript(textTracks);
		}

		BuildOEmbedViewModel(startTime, video.Data, oEmbed.Data, archiveVideo, member);

		archiveVideo.Views++;
		await _repository.UpdateAsync(archiveVideo);

		return Page();
	}

	private async Task<(HttpResponse<Video>, HttpResponse<GetAllTextTracksResponse>, ArchiveVideo?, ApplicationUser)> GetDataAsync(string videoId)
	{
		var videoTask = _getVideoService.ExecuteAsync(videoId);
		var textTracksTask = _getAllTextTracksService.ExecuteAsync(videoId);
		var videoSpec = new ArchiveVideoByVideoIdFullAggregateSpec(videoId);
		var archiveVideoTask = _repository.FirstOrDefaultAsync(videoSpec);
		var currentUserName = User.Identity!.Name;
		var applicationUserTask = _userManager.FindByNameAsync(currentUserName);

		var task = Task.WhenAll(videoTask, textTracksTask, archiveVideoTask, applicationUserTask);
		try
		{
			await task;
		}
		catch (Exception)
		{
			if (task.Exception != null)
			{
				throw task.Exception;
			}

			throw;
		}

		return (videoTask.Result, textTracksTask.Result, archiveVideoTask.Result, applicationUserTask.Result);
	}

	private async Task<(HttpResponse<OEmbed>, Member?)> GetMoreDataAsync(string videoLink, string userId)
	{
		var oEmbedTask = _getOEmbedVideoService.ExecuteAsync(videoLink);
		var memberSpec = new MemberByUserIdWithFavoriteArchiveVideosSpec(userId);
		var memberTask = _memberRepository.FirstOrDefaultAsync(memberSpec);

		var task = Task.WhenAll(oEmbedTask, memberTask);
		try
		{
			await task;
		}
		catch (Exception)
		{
			if (task.Exception != null)
			{
				throw task.Exception;
			}

			throw;
		}

		return (oEmbedTask.Result, memberTask.Result);
	}

	private async Task GetTranscript(HttpResponse<GetAllTextTracksResponse> textTracks)
	{
		var textTrackUrl = textTracks.Data.Data.First().Link;
		var textTrackResponse = (await _httpClient.GetAsync(textTrackUrl));
		if (textTrackResponse.IsSuccessStatusCode)
		{
			var vtt = await textTrackResponse.Content.ReadAsStringAsync();
			var currentURL = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;
			Transcript = _vttService.Parse(vtt, currentURL);
		}
	}

	private void BuildOEmbedViewModel(string? startTime, Video video, OEmbed oEmbed, ArchiveVideo archiveVideo, Member member)
	{
		var progress = archiveVideo.MembersVideoProgress.FirstOrDefault(x => x.ArchiveVideoId == archiveVideo.Id && x.MemberId == member.Id);
		OEmbedViewModel = new OEmbedViewModel(oEmbed);
		OEmbedViewModel.VideoId = int.Parse(archiveVideo.VideoId!);
		OEmbedViewModel.Name = archiveVideo.Title;

		archiveVideo.CreateMdComments(_markdownService);
		OEmbedViewModel.Comments = _mapper.Map<List<VideoCommentDto>>(archiveVideo.Comments);

		OEmbedViewModel.Password = video.Password;
		OEmbedViewModel.DescriptionMd = _markdownService.RenderHTMLFromMD(archiveVideo.Description);
		OEmbedViewModel.Description = archiveVideo.Description;
		OEmbedViewModel
			.AddStartTime(startTime)
			.BuildHtml(video.Link);
		OEmbedViewModel.IsMemberFavorite = member.FavoriteArchiveVideos.Any(fav => fav.ArchiveVideoId == archiveVideo.Id);
		OEmbedViewModel.IsMemberWatched = progress is { VideoWatchedStatus: VideoWatchedStatus.Watched };
		OEmbedViewModel.MemberFavoritesCount = archiveVideo.MemberFavorites.Count();
	}
}
