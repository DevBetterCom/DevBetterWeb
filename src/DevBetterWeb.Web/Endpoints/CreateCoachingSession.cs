using System;
using System.Globalization;
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
public class CreateCoachingSession : EndpointBaseAsync
	.WithRequest<CoachingSessionAddEditDto>
	.WithResult<ActionResult<CoachingSessionAddEditDto>>
{
	private readonly IRepository<CoachingSession> _repository;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IRepository<Member> _memberRepository;

	public CreateCoachingSession(
		IRepository<CoachingSession> repository,
		UserManager<ApplicationUser> userManager,
		IRepository<Member> memberRepository)
	{
		_repository = repository;
		_userManager = userManager;
		_memberRepository = memberRepository;
	}

	[HttpPost("coaching-session/create")]
	public override async Task<ActionResult<CoachingSessionAddEditDto>> HandleAsync([FromBody] CoachingSessionAddEditDto request, CancellationToken cancellationToken = default)
	{
		var currentUserName = User.Identity!.Name;
		var applicationUser = await _userManager.FindByNameAsync(currentUserName);

		var memberSpec = new MemberByUserIdWithFavoriteArchiveVideosSpec(applicationUser.Id);
		var member = await _memberRepository.FirstOrDefaultAsync(memberSpec, cancellationToken);
		if (member is null)
		{
			return Unauthorized();
		}

		CultureInfo provider = new CultureInfo("en-US");
		var coachingSessionToSave = new CoachingSession(DateTime.ParseExact(request.StartAt, "dd-MM-yyyy HH:mm:ss", provider));
		await _repository.AddAsync(coachingSessionToSave, cancellationToken);

		return Ok(request);
	}
}
