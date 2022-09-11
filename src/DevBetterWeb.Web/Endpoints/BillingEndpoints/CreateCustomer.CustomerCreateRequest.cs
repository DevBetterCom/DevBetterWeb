using System.ComponentModel.DataAnnotations;

namespace DevBetterWeb.Web.Endpoints;

public class CustomerCreateRequest
{
	[Required]
	public string? Email { get; set; }
}
