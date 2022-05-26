using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Vimeo.Services.VideoServices;
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

  private readonly GetOEmbedVideoService _getOEmbedVideoService;
  private readonly GetVideoService _getVideoService;
  private readonly IRepository<ArchiveVideo> _repository;
  private readonly IMarkdownService _markdownService;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IRepository<Member> _memberRepository;

  public DetailsModel(IMarkdownService markdownService, GetOEmbedVideoService getOEmbedVideoService, GetVideoService getVideoService, IRepository<ArchiveVideo> repository, UserManager<ApplicationUser> userManager, IRepository<Member> memberRepository)
  {
    _markdownService = markdownService;
    _getVideoService = getVideoService;
    _repository = repository;
    _getOEmbedVideoService = getOEmbedVideoService;
    _userManager = userManager;
    _memberRepository = memberRepository;
  }

  public async Task<IActionResult> OnGet(string videoId, string? startTime = null)
  {
    var video = await _getVideoService.ExecuteAsync(videoId);
    if (video?.Data == null) return NotFound($"Video Not Found {videoId}");

    var oEmbed = await _getOEmbedVideoService.ExecuteAsync(video.Data.Link);
    if (oEmbed?.Data == null) return NotFound($"Video Not Found {videoId}");

    var videoSpec = new ArchiveVideoByVideoIdSpec(videoId);
    var archiveVideo = await _repository.GetBySpecAsync(videoSpec);
    if (archiveVideo == null) return NotFound($"Video Not Found {videoId}");

    archiveVideo.Views++;
    await _repository.UpdateAsync(archiveVideo);

    var currentUserName = User.Identity!.Name;
    var applicationUser = await _userManager.FindByNameAsync(currentUserName);

    var memberSpec = new MemberByUserIdWithFavoriteArchiveVideosSpec(applicationUser.Id);
    var member = await _memberRepository.GetBySpecAsync(memberSpec);

    OEmbedViewModel = new OEmbedViewModel(oEmbed?.Data);
    OEmbedViewModel.VideoId = int.Parse(archiveVideo.VideoId);
    OEmbedViewModel.Name = archiveVideo.Title;
    OEmbedViewModel.Password = video?.Data?.Password;
    OEmbedViewModel.DescriptionMd = _markdownService.RenderHTMLFromMD(archiveVideo.Description);
    OEmbedViewModel.Description = archiveVideo.Description;
    OEmbedViewModel
      .AddStartTime(startTime)
      .BuildHtml(video?.Data?.Link);
    OEmbedViewModel.IsMemberFavorite = member.FavoriteArchiveVideos.Any(fav => fav.ArchiveVideoId == archiveVideo.Id);
	OEmbedViewModel.MemberFavoritesCount = archiveVideo.MemberFavorites.Count();
    return Page();
  }
}
