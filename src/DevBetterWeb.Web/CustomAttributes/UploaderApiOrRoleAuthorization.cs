using Microsoft.AspNetCore.Mvc;

namespace DevBetterWeb.Web.CustomAttributes;

public class UploaderApiOrRoleAuthorization : TypeFilterAttribute
{
	public UploaderApiOrRoleAuthorization(params string[] roles) : base(typeof(UploaderApiAuthorizationFilter))
	{
		Arguments = new object[] { roles };
	}
}
