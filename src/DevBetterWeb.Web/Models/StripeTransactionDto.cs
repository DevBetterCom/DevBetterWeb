using System;

namespace DevBetterWeb.Web.Models;

public class StripeTransactionDto
{
	public DateTime CreatedDate { get; set; }
	public string CardHolder { get; set; } = string.Empty;
	public long Amount { get; set; }
}
