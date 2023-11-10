using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using DevBetterWeb.Core;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Web.CustomAttributes;

public class UploaderApiAuthorizationFilter : IAuthorizationFilter
{
	private readonly string _expectedApiKey;
	private readonly string[] _allowedRoles;

	public UploaderApiAuthorizationFilter(IOptions<ApiSettings> apiSettings, params string[] roles)
	{
		_expectedApiKey = apiSettings.Value.ApiKey;
		_allowedRoles = roles;
	}

	public void OnAuthorization(AuthorizationFilterContext context)
	{
		var apiKey = context.HttpContext.Request.Headers[Constants.ConfigKeys.ApiKey];
		if (string.Equals(_expectedApiKey, apiKey.ToString(), StringComparison.CurrentCultureIgnoreCase))
		{
			return;
		}

		var user = context.HttpContext.User;
		List<string> userRoles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

		var result = false;
		if (userRoles.Any())
		{
			foreach (var userRole in userRoles)
			{
				if (!string.IsNullOrEmpty(userRole))
				{
					foreach (var role in _allowedRoles)
					{
						result = result || IsValidRole(userRole, role);
					}
				}
			}
		}

		if (!result)
		{
			context.Result = new UnauthorizedResult();
		}
	}

	private static bool IsValidRole(string tokenRole, string targetRole)
	{
		List<string> roles = tokenRole.Split(',').Where(r => string.Equals(r, targetRole, StringComparison.CurrentCultureIgnoreCase)).ToList();

		return roles.Count >= 1;
	}
}
