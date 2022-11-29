using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Web.Endpoints;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
public class MarkFavoriteVideo : EndpointBaseAsync
	.WithRequest<string>
	.WithActionResult
{
	private readonly IRepository<ArchiveVideo> _archiveVideoRepository;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IRepository<Member> _memberRepository;

	public MarkFavoriteVideo(IRepository<ArchiveVideo> archiveVideoRepository, UserManager<ApplicationUser> userManager,
		IRepository<Member> memberRepository)
	{
		_archiveVideoRepository = archiveVideoRepository;
		_userManager = userManager;
		_memberRepository = memberRepository;
	}

	[HttpPut("videos/favorite-video/{vimeoVideoId}")]
	public override async Task<ActionResult> HandleAsync([FromRoute] string vimeoVideoId, CancellationToken cancellationToken = default)
	{
		var currentUserName = User.Identity!.Name;
		var applicationUser = await _userManager.FindByNameAsync(currentUserName!);

		var memberSpec = new MemberByUserIdWithFavoriteArchiveVideosSpec(applicationUser!.Id);
		var member = await _memberRepository.FirstOrDefaultAsync(memberSpec, cancellationToken);

		if (member is null)
		{
			return Unauthorized();
		}

		var videoSpec = new ArchiveVideoByVideoIdSpec(vimeoVideoId);
		var archiveVideo = await _archiveVideoRepository.FirstOrDefaultAsync(videoSpec, cancellationToken);
		if (archiveVideo == null) return NotFound($"Video Not Found {vimeoVideoId}");

		if (member.FavoriteArchiveVideos.Any(v => v.ArchiveVideoId == archiveVideo.Id))
		{
			member.RemoveFavoriteArchiveVideo(archiveVideo);
		}
		else
		{
			member.AddFavoriteArchiveVideo(archiveVideo);
		}

		await _memberRepository.UpdateAsync(member, cancellationToken);

		return Ok(new MarkFavoriteVideoResponse(true, "Done!"));
	}
}
