using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

	public DetailsModel(IMapper mapper, IMarkdownService markdownService, GetOEmbedVideoService getOEmbedVideoService, 
	  GetVideoService getVideoService, IRepository<ArchiveVideo> repository, UserManager<ApplicationUser> userManager, 
	  IRepository<Member> memberRepository, GetAllTextTracksService getAllTextTracksService, HttpClient httpClient)
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
	}

  public async Task<IActionResult> OnGet(string videoId, string? startTime = null)
  {
    var videoTask = _getVideoService.ExecuteAsync(videoId);
    var textTracksTask = _getAllTextTracksService.ExecuteAsync(videoId);

    var video = await videoTask;
    if (video?.Data == null) return NotFound($"Video Not Found {videoId}");

    var textTracks = await textTracksTask;
    if (textTracks?.Data != null)
    {
      var textTrackUrl = textTracks.Data.Data.First().Link;
      var textTrackResponse = (await _httpClient.GetAsync(textTrackUrl));
      if (textTrackResponse.IsSuccessStatusCode)
      {
        Transcript = await textTrackResponse.Content.ReadAsStringAsync();
      }
    }

    var oEmbed = await _getOEmbedVideoService.ExecuteAsync(video.Data.Link);
    if (oEmbed?.Data == null) return NotFound($"Video Not Found {videoId}");

    var videoSpec = new ArchiveVideoByVideoIdWithMemberFavoritesAndCommentsSpec(videoId);
    var archiveVideo = await _repository.FirstOrDefaultAsync(videoSpec);
    if (archiveVideo == null) return NotFound($"Video Not Found {videoId}");

    archiveVideo.Views++;
    await _repository.UpdateAsync(archiveVideo);

    var currentUserName = User.Identity!.Name;
    var applicationUser = await _userManager.FindByNameAsync(currentUserName);

    var memberSpec = new MemberByUserIdWithFavoriteArchiveVideosSpec(applicationUser.Id);
    var member = await _memberRepository.FirstOrDefaultAsync(memberSpec);
    if (member == null) return NotFound($"Member Not Found {applicationUser.Id}");

    var spec = new ArchiveVideoByVideoIdWithProgressSpec(videoId);
    var existVideoWithProgress = await _repository.FirstOrDefaultAsync(spec);
    var progress = existVideoWithProgress!.MembersVideoProgress.FirstOrDefault(x => x.ArchiveVideoId == existVideoWithProgress.Id && x.MemberId == member.Id);

		OEmbedViewModel = new OEmbedViewModel(oEmbed.Data);
    OEmbedViewModel.VideoId = int.Parse(archiveVideo.VideoId!);
    OEmbedViewModel.Name = archiveVideo.Title;

    archiveVideo.CreateMdComments(_markdownService);
		OEmbedViewModel.Comments = _mapper.Map<List<VideoCommentDto>>(archiveVideo.Comments);

    OEmbedViewModel.Password = video.Data.Password;
    OEmbedViewModel.DescriptionMd = _markdownService.RenderHTMLFromMD(archiveVideo.Description);
    OEmbedViewModel.Description = archiveVideo.Description;
    OEmbedViewModel
      .AddStartTime(startTime)
      .BuildHtml(video.Data.Link);
    OEmbedViewModel.IsMemberFavorite = member.FavoriteArchiveVideos.Any(fav => fav.ArchiveVideoId == archiveVideo.Id);
    OEmbedViewModel.IsMemberWatched = progress is { VideoWatchedStatus: VideoWatchedStatus.Watched };
		OEmbedViewModel.MemberFavoritesCount = archiveVideo.MemberFavorites.Count();

    return Page();
  }
}
