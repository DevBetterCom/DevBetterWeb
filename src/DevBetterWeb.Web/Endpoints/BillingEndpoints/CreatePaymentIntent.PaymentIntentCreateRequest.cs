using System.ComponentModel.DataAnnotations;

namespace DevBetterWeb.Web.Endpoints;

public class PaymentIntentCreateRequest
{
	[Required]
	public string? SubscriptionPriceId { get; set; }
	[Required]
	public string? Currency { get; set; }
	[Required]
	public string? PaymentMethodType { get; set; }
}
