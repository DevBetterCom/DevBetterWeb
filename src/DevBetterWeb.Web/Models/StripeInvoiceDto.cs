using System;

namespace DevBetterWeb.Web.Models;

public class StripeInvoiceDto
{
	public string Id { get; set; } = string.Empty;
	public string InvoicePdf  { get; set; } = string.Empty;
	public string Number { get; set; } = string.Empty;
	public bool IsPaid { get; set; }
	public bool IsPaidOutOfBand { get; set; }
	public string Status { get; set; } = string.Empty;
	public DateTime? FinalizedAt { get; set; }
	public DateTime? PaidAt { get; set; }
	public long? AmountDue { get; set; }
	public long? AmountPaid { get; set; }
	public long? AmountRemaining { get; set; }
	public long? Total { get; set; }
	public int AttemptCount { get; set; }
	public string Currency { get; set; } = string.Empty;
	public string CustomerId { get; set; } = string.Empty;
	public string CustomerName { get; set; } = string.Empty;
	public string CustomerEmail { get; set; } = string.Empty;
	public string CustomerPhone { get; set; } = string.Empty;
}
