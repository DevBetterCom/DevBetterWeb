using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DevBetterWeb.Web.Endpoints;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS)]
public class EditCoachingSession : EndpointBaseAsync
	.WithRequest<CoachingSessionAddEditDto>
	.WithResult<ActionResult<CoachingSessionAddEditDto>>
{
	private readonly IRepository<CoachingSession> _repository;

	public EditCoachingSession(
		IRepository<CoachingSession> repository)
	{
		_repository = repository;
	}

	[HttpPut("coaching-session/edit")]
	public override async Task<ActionResult<CoachingSessionAddEditDto>> HandleAsync([FromBody] CoachingSessionAddEditDto request, CancellationToken cancellationToken = default)
	{
		CultureInfo provider = new CultureInfo("en-US");
		var coachingSessionToSave = new CoachingSession(DateTime.ParseExact(request.StartAt, "dd-MM-yyyy HH:mm:ss", provider));
		coachingSessionToSave.Id = request.Id!.Value;

		await _repository.UpdateAsync(coachingSessionToSave, cancellationToken);

		return Ok(request);
	}
}
