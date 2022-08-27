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
public class SubmitCommentReply : EndpointBaseAsync
	.WithRequest<CommentReplyRequest>
	.WithActionResult
{
	private readonly IRepository<ArchiveVideo> _archiveVideoRepository;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IRepository<Member> _memberRepository;

	public SubmitCommentReply(IRepository<ArchiveVideo> archiveVideoRepository, UserManager<ApplicationUser> userManager,
		IRepository<Member> memberRepository)
	{
		_archiveVideoRepository = archiveVideoRepository;
		_userManager = userManager;
		_memberRepository = memberRepository;
	}

	[HttpPost("videos/submit-comment-reply")]
	public override async Task<ActionResult> HandleAsync([FromForm] CommentReplyRequest request, CancellationToken cancellationToken = default)
	{
		var userId = _userManager.GetUserId(User);
		var memberByUserSpec = new MemberByUserIdSpec(userId);
		var member = await _memberRepository.FirstOrDefaultAsync(memberByUserSpec, cancellationToken);
		if (member == null)
		{
			return Unauthorized();
		}

		var spec = new ArchiveVideoByVideoIdSpec(request.VideoId!);
		var existVideo = await _archiveVideoRepository.FirstOrDefaultAsync(spec, cancellationToken);
		if (existVideo == null)
		{
			return NotFound("Video not found!");
		}
		existVideo.AddComment(new VideoComment(member.Id, existVideo.Id, request.CommentReplyToSubmit));
		await _archiveVideoRepository.UpdateAsync(existVideo, cancellationToken);


		return Ok(new SubmitCommentReplyResponse(true, "Your message successfully sent!"));
	}
}
