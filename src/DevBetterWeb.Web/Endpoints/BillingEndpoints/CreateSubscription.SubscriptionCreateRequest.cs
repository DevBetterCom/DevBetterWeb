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
	public string? CardNumber { get; set; }
	public int CardExpMonth { get; set; }
	public int CardExpYear { get; set; }
	public string? CardCvc { get; set; }
}
