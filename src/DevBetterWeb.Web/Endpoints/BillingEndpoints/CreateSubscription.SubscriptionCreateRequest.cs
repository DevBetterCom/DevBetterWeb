using System.ComponentModel.DataAnnotations;

namespace DevBetterWeb.Web.Endpoints;

public class SubscriptionCreateRequest
{
	[Required]
	public string? CustomerEmail { get; set; }
	[Required]
	public string? PriceId { get; set; }
	[Required]
	public string? PaymentMethodId { get; set; }
}
