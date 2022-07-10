using System;
using DevBetterWeb.Core;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Web.CustomAttributes;

public class IntegrationApiAuthorizationFilter : IAuthorizationFilter
{
	private readonly string _expectedApiKey;

	public IntegrationApiAuthorizationFilter(IOptions<ApiSettings> apiSettings)
	{
		_expectedApiKey = apiSettings.Value.ApiKey;
	}

	public void OnAuthorization(AuthorizationFilterContext context)
	{
		var apiKey = context.HttpContext.Request.Headers[Constants.ConfigKeys.ApiKey];

		if (!string.Equals(_expectedApiKey, apiKey.ToString(), StringComparison.CurrentCultureIgnoreCase))
		{
			context.Result = new UnauthorizedResult();
		}
	}
}
