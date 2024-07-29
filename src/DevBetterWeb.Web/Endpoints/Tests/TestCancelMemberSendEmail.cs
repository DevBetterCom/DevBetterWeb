using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DevBetterWeb.Web.Endpoints;

public class TestCancelMemberSendEmail : EndpointBaseAsync
	.WithoutRequest
	.WithActionResult
{
	private readonly IMemberCancellationService _memberCancellationService;

	public TestCancelMemberSendEmail(IMemberCancellationService memberCancellationService)
	{
		_memberCancellationService = memberCancellationService;
	}

	[HttpGet("tests/member-cancellation-send-email")]
	public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
	{
		await _memberCancellationService.SendCancellationEmailAsync("to@example.com");

		return Ok();
	}
}
