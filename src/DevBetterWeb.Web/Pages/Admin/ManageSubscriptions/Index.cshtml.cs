using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Admin.ManageSubscriptions;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class IndexModel : PageModel
{
	private readonly ISubscriptionHandlerService _subscriptionHandlerService;
	private readonly IMapper _mapper;

	public List<StripeSubscriptionDto> Subscriptions { get; private set; } = new();

	[TempData]
	public string? StatusMessage { get; set; }

	public IndexModel(ISubscriptionHandlerService subscriptionHandlerService, IMapper mapper)
	{
		_subscriptionHandlerService = subscriptionHandlerService;
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
}
