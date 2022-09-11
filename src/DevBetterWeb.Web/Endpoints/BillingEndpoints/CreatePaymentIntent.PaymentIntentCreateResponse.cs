namespace DevBetterWeb.Web.Endpoints;

public class PaymentIntentCreateResponse
{
	public string? Id { get; set; }
	public string? ClientSecret { get; set; }
	public string? ErrorMessage { get; set; }


	public PaymentIntentCreateResponse(string id, string clientSecret)
	{
		Id = id;
		ClientSecret = clientSecret;
	}

	public PaymentIntentCreateResponse(string? errorMessage)
	{
		ErrorMessage = errorMessage;
	}
}
