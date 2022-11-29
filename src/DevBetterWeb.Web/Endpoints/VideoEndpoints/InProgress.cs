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
public class InProgress : EndpointBaseAsync
	.WithRequest<string>
	.WithActionResult
{
	private readonly IRepository<ArchiveVideo> _archiveVideoRepository;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IRepository<Member> _memberRepository;

	public InProgress(IRepository<ArchiveVideo> archiveVideoRepository, UserManager<ApplicationUser> userManager,
		IRepository<Member> memberRepository)
	{
		_archiveVideoRepository = archiveVideoRepository;
		_userManager = userManager;
		_memberRepository = memberRepository;
	}

	[HttpGet("videos/inprogress/{videoId}")]
	public override async Task<ActionResult> HandleAsync(string videoId, CancellationToken cancellationToken = default)
	{
		var userId = _userManager.GetUserId(User);
		var memberByUserSpec = new MemberByUserIdSpec(userId!);
		var member = await _memberRepository.FirstOrDefaultAsync(memberByUserSpec, cancellationToken);
		if (member == null)
		{
			return Unauthorized();
		}

		var spec = new ArchiveVideoByVideoIdWithProgressSpec(videoId);
		var existVideo = await _archiveVideoRepository.FirstOrDefaultAsync(spec, cancellationToken);
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
			if (progress.VideoWatchedStatus == VideoWatchedStatus.Watched)
			{
				return Ok(new InProgressResponse(true, "Done!"));
			}
			progress.SetToInProgress();
		}

		await _archiveVideoRepository.UpdateAsync(existVideo, cancellationToken);

		return Ok(new InProgressResponse(true, "Done!"));
	}
}
