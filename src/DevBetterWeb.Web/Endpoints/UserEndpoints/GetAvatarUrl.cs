using Ardalis.ApiEndpoints;
using DevBetterWeb.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevBetterWeb.Web.UserEndpoints;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
public class GetAvatarUrl : EndpointBaseSync
	.WithoutRequest
	.WithActionResult<string>
{

	[HttpGet("users/avatar-url")]
	public override ActionResult<string> Handle()
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (string.IsNullOrEmpty(userIdClaim))
		{
			return Unauthorized("User ID is not available.");
		}

		return Ok(string.Format(Constants.AVATAR_IMGURL_FORMAT_STRING, userIdClaim));
	}
}
