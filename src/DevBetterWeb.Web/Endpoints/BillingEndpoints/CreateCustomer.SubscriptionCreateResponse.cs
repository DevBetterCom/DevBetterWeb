namespace DevBetterWeb.Web.Endpoints;

public class CustomerCreateResponse
{
	public string? CustomerId { get; set; }
	public string? Email { get; set; }
	public string? ErrorMessage { get; set; }

	public CustomerCreateResponse(string customerId, string email)
	{
		CustomerId = customerId;
		Email = email;
	}
	
	public CustomerCreateResponse(string? errorMessage)
	{
		ErrorMessage = errorMessage;
	}
}
