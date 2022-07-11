using Microsoft.AspNetCore.Mvc;

namespace DevBetterWeb.Web.CustomAttributes;

public class UploaderApiAuthorization : TypeFilterAttribute
{
	public UploaderApiAuthorization() : base(typeof(UploaderApiAuthorizationFilter))
	{
	}
}
