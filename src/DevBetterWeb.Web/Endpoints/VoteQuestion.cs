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
public class VoteQuestion : EndpointBaseAsync
	.WithRequest<VoteQuestionRequestDto>
	.WithResult<ActionResult<VoteQuestionRequestDto>>
{
	private readonly IRepository<Question> _repository;
	private readonly IRepository<QuestionVote> _questionVoteRepository;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IRepository<Member> _memberRepository;

	public VoteQuestion(
		IRepository<Question> repository,
		IRepository<QuestionVote> questionVoteRepository,
		UserManager<ApplicationUser> userManager,
		IRepository<Member> memberRepository)
	{
		_repository = repository;
		_questionVoteRepository = questionVoteRepository;
		_userManager = userManager;
		_memberRepository = memberRepository;
	}

	[HttpPost("coaching-session/vote-question")]
	public override async Task<ActionResult<VoteQuestionRequestDto>> HandleAsync([FromBody] VoteQuestionRequestDto request, CancellationToken cancellationToken = default)
	{
		var currentUserName = User.Identity!.Name;
		var applicationUser = await _userManager.FindByNameAsync(currentUserName);

		var memberSpec = new MemberByUserIdWithFavoriteArchiveVideosSpec(applicationUser.Id);
		var member = await _memberRepository.FirstOrDefaultAsync(memberSpec, cancellationToken);
		if (member is null)
		{
			return Unauthorized();
		}

		var spec = new QuestionWithVotesSpec(request.QuestionId);
		var question = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
		if (question is null)
		{
			return NotFound();
		}

		question.AddRemoveVote(member.Id);
		await _repository.UpdateAsync(question, cancellationToken);

		return Ok(request);
	}
}
