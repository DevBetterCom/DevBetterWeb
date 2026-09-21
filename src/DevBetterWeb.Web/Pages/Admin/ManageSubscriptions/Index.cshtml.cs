using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;

namespace DevBetterWeb.Web.Pages.Admin.ManageSubscriptions;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class IndexModel : PageModel
{
	private readonly ISubscriptionHandlerService _subscriptionHandlerService;
	private readonly IWebhookHandlerService _webhookHandlerService;
	private readonly IMapper _mapper;

	public List<StripeSubscriptionDto> Subscriptions { get; private set; } = new();

	[TempData]
	public string? StatusMessage { get; set; }

	public IndexModel(ISubscriptionHandlerService subscriptionHandlerService,
		IWebhookHandlerService webhookHandlerService,
		IMapper mapper)
	{
		_subscriptionHandlerService = subscriptionHandlerService;
		_webhookHandlerService = webhookHandlerService;
		_mapper = mapper;
	}

	public async Task OnGetAsync()
	{
		var subscriptions = await _subscriptionHandlerService.ListBillableAsync(HttpContext.RequestAborted);
		Subscriptions = _mapper.Map<List<StripeSubscriptionDto>>(subscriptions)
			.OrderBy(s => s.CustomerEmail)
			.ThenBy(s => s.Id)
			.ToList();
	}

	public Task<IActionResult> OnPostPauseAsync(string subscriptionId)
		=> ExecuteActionAsync(subscriptionId, id => _subscriptionHandlerService.PauseAsync(id, HttpContext.RequestAborted),
			"paused (Stripe will not charge until resumed)");

	public Task<IActionResult> OnPostResumeAsync(string subscriptionId)
		=> ExecuteActionAsync(subscriptionId, id => _subscriptionHandlerService.ResumeAsync(id, HttpContext.RequestAborted),
			"resumed");

	public Task<IActionResult> OnPostCancelAsync(string subscriptionId)
		=> ExecuteActionAsync(subscriptionId, id => _subscriptionHandlerService.CancelAtPeriodEndAsync(id, HttpContext.RequestAborted),
			"set to cancel at period end");

	public Task<IActionResult> OnPostCancelNowAsync(string subscriptionId)
		=> ExecuteActionAsync(subscriptionId, id => _subscriptionHandlerService.CancelImmediatelyAsync(id, HttpContext.RequestAborted),
			"canceled immediately");

	// Replays invoice.paid processing (new member invitation or renewal) for events Stripe could not deliver.
	public Task<IActionResult> OnPostReplayPaidInvoiceAsync(string invoiceId)
		=> ExecuteReplayAsync(invoiceId, "invoice", () => _webhookHandlerService.ReprocessPaidInvoiceAsync(invoiceId.Trim()));

	// Replays customer.subscription.deleted processing (member role removal) for events Stripe could not deliver.
	public Task<IActionResult> OnPostReplaySubscriptionEndedAsync(string subscriptionId)
		=> ExecuteReplayAsync(subscriptionId, "subscription", () => _webhookHandlerService.ReprocessSubscriptionEndedAsync(subscriptionId.Trim()));

	private async Task<IActionResult> ExecuteReplayAsync(string id, string idKind, Func<Task<string>> replay)
	{
		if (string.IsNullOrWhiteSpace(id))
		{
			StatusMessage = $"No {idKind} id provided.";
			return RedirectToPage();
		}

		try
		{
			StatusMessage = await replay();
		}
		catch (Exception exception)
		{
			StatusMessage = $"Error replaying {idKind} {id}: {exception.Message}";
		}

		return RedirectToPage();
	}

	private async Task<IActionResult> ExecuteActionAsync(string subscriptionId,
		Func<string, Task<Subscription>> action, string successVerb)
	{
		if (string.IsNullOrWhiteSpace(subscriptionId))
		{
			StatusMessage = "No subscription id provided.";
			return RedirectToPage();
		}

		try
		{
			await action(subscriptionId);
			StatusMessage = $"Subscription {subscriptionId} {successVerb}.";
		}
		catch (StripeException exception)
		{
			StatusMessage = $"Stripe error for {subscriptionId}: {exception.Message}";
		}

		return RedirectToPage();
	}
}
