using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Web.Endpoints;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS)]
public class CreateQuestion : EndpointBaseAsync
	.WithRequest<NewQuestionRequestDto>
	.WithResult<ActionResult<NewQuestionRequestDto>>
{
	private readonly IRepository<CoachingSession> _repository;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IRepository<Member> _memberRepository;

	public CreateQuestion(
		IRepository<CoachingSession> repository,
		UserManager<ApplicationUser> userManager,
		IRepository<Member> memberRepository)
	{
		_repository = repository;
		_userManager = userManager;
		_memberRepository = memberRepository;
	}

	[HttpPost("coaching-session/create-question")]
	public override async Task<ActionResult<NewQuestionRequestDto>> HandleAsync([FromBody] NewQuestionRequestDto request, CancellationToken cancellationToken = default)
	{
		var currentUserName = User.Identity!.Name;
		var applicationUser = await _userManager.FindByNameAsync(currentUserName!);

		var memberSpec = new MemberByUserIdWithFavoriteArchiveVideosSpec(applicationUser!.Id);
		var member = await _memberRepository.FirstOrDefaultAsync(memberSpec, cancellationToken);
		if (member is null)
		{
			return Unauthorized();
		}

		var spec = new CoachingSessionWithQuestionsSpec(request.CoachingSessionId);
		var coachingSession = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
		if (coachingSession is null)
		{
			return NotFound();
		}
		var newQuestion = new Question(member.Id, request.QuestionText);
		newQuestion.AddRemoveVote(member.Id);
		coachingSession.AddQuestion(newQuestion);

		await _repository.UpdateAsync(coachingSession, cancellationToken);

		return Ok(request);
	}
}
