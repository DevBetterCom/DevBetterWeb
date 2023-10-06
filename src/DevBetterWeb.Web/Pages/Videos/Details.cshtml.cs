using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Pages.Admin.Videos;
using Microsoft.AspNetCore.Authorization;
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
	private readonly IMapper _mapper;
	private readonly IMarkdownService _markdownService;
	private readonly IRepository<Member> _memberRepository;
	private readonly IVideoDetailsService _videoDetailsService;

	public DetailsModel(
		IMapper mapper,
		IMarkdownService markdownService,
		GetOEmbedVideoService getOEmbedVideoService,
		IRepository<Member> memberRepository,
		IVideoDetailsService videoDetailsService)
	{
		_mapper = mapper;
		_markdownService = markdownService;
		_getOEmbedVideoService = getOEmbedVideoService;
		_memberRepository = memberRepository;
		_videoDetailsService = videoDetailsService;
	}

	public async Task<IActionResult> OnGet(string videoId, string? startTime = null)
	{
		var currentUserName = User.Identity!.Name;
		var currentVideoURL = $"{Request.Scheme}://{Request.Host.Value}/Videos/Details/{videoId}";
		var (video, transcript, archiveVideo, applicationUser) = await _videoDetailsService.GetDataAsync(videoId, currentUserName, currentVideoURL);
		if (video?.Data == null) return NotFound($"Video Not Found {videoId}");
		if (archiveVideo == null) return NotFound($"Video Not Found {videoId}");

		var (oEmbed, member) = await GetMoreDataAsync(video.Data.Link, applicationUser.Id);
		if (oEmbed?.Data == null) return NotFound($"Video Not Found {videoId}");
		if (member == null) return NotFound($"Member Not Found {applicationUser.Id}");
	
		Transcript = transcript;

		BuildOEmbedViewModel(startTime, video.Data, oEmbed.Data, archiveVideo, member);

		await _videoDetailsService.IncrementViewsAndUpdate(archiveVideo);

		return Page();
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
