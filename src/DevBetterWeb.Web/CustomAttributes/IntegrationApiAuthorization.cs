using Microsoft.AspNetCore.Mvc;

namespace DevBetterWeb.Web.CustomAttributes;

public class IntegrationApiAuthorization : TypeFilterAttribute
{
	public IntegrationApiAuthorization() : base(typeof(IntegrationApiAuthorizationFilter))
	{
	}
}
