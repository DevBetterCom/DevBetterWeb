using System;

namespace DevBetterWeb.Web.Models;

public class StripeSubscriptionDto
{
	public string Id { get; set; } = string.Empty;
	public string Status { get; set; } = string.Empty;
	public string CustomerId { get; set; } = string.Empty;
	public string CustomerEmail { get; set; } = string.Empty;
	public string PlanName { get; set; } = string.Empty;
	public decimal Amount { get; set; }
	public string Currency { get; set; } = string.Empty;
	public string Interval { get; set; } = string.Empty;
	public DateTime CurrentPeriodEnd { get; set; }
	public bool CancelAtPeriodEnd { get; set; }
	public bool IsPaused { get; set; }
}
