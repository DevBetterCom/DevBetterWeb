namespace DevBetterWeb.Web.Endpoints;

public class SubscriptionCreateResponse
{
	public string? Id { get; set; }
	public string? Status { get; set; }
	public string? LatestInvoicePaymentIntentStatus { get; set; }
	public string? LatestInvoicePaymentIntentClientSecret { get; set; }
	public string? ErrorMessage { get; set; }

	public SubscriptionCreateResponse(string id, string status, string latestInvoicePaymentIntentStatus, string latestInvoicePaymentIntentClientSecret, string? errorMessage)
	{
		Id = id;
		Status = status;
		LatestInvoicePaymentIntentStatus = latestInvoicePaymentIntentStatus;
		LatestInvoicePaymentIntentClientSecret = latestInvoicePaymentIntentClientSecret;
		ErrorMessage = errorMessage;
	}

	public SubscriptionCreateResponse(string? errorMessage)
	{
		ErrorMessage = errorMessage;
	}
}
