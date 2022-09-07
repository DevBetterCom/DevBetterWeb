using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Core.ValueObjects;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Pages.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Web.Pages.Admin;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class UserModel : PageModel
{
	[BindProperty] public UserPersonalUpdateModel UserPersonalUpdateModel { get; set; } = new UserPersonalUpdateModel();
	[BindProperty] public UserLinksUpdateModel UserLinksUpdateModel { get; set; } = new UserLinksUpdateModel();

	public List<StripeTransactionDto> Transactions = new List<StripeTransactionDto>();

	private readonly ILogger<UserModel> _logger;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly IUserRoleMembershipService _userRoleMembershipService;
	private readonly IMemberRegistrationService _memberRegistrationService;
	private readonly IRepository<Member> _memberRepository;
	private readonly IRepository<MemberSubscription> _subscriptionRepository;
	private readonly IRepository<MemberSubscriptionPlan> _subscriptionPlanRepository;
	private readonly IUserEmailConfirmationService _userEmailConfirmationService;
	private readonly IIssuingHandlerTransactionListService _issuingHandlerTransactionListService;
	private readonly IMapper _mapper;

	public UserModel(ILogger<UserModel> logger,
		UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			IUserRoleMembershipService userRoleMembershipService,
			IMemberRegistrationService memberRegistrationService,
			IRepository<Member> memberRepository,
			IRepository<MemberSubscription> subscriptionRepository,
			IRepository<MemberSubscriptionPlan> subscriptionPlanRepository,
			IUserEmailConfirmationService userEmailConfirmationService,
			IIssuingHandlerTransactionListService issuingHandlerTransactionListService,
			IMapper mapper)
	{
		_logger = logger;
		_userManager = userManager;
		_roleManager = roleManager;
		_userRoleMembershipService = userRoleMembershipService;
		_memberRegistrationService = memberRegistrationService;
		_memberRepository = memberRepository;
		_subscriptionRepository = subscriptionRepository;
		_subscriptionPlanRepository = subscriptionPlanRepository;
		_userEmailConfirmationService = userEmailConfirmationService;
		_issuingHandlerTransactionListService = issuingHandlerTransactionListService;
		_mapper = mapper;
	}


	public IdentityUser? IdentityUser { get; set; }
	public List<IdentityRole> Roles { get; set; } = new List<IdentityRole>();
	public List<SelectListItem> RolesNotAssignedToUser { get; set; } = new List<SelectListItem>();
	public SubscriptionDTO Subscription { get; set; } = new SubscriptionDTO();
	public List<SubscriptionDTO> Subscriptions { get; set; } = new List<SubscriptionDTO>();
	public double TotalDaysInAllSubscriptions { get; set; }
	public EmailConfirmationModel EmailConfirmation { get; set; } = new EmailConfirmationModel();
	public List<MemberSubscriptionPlan> MemberSubscriptionPlans { get; set; } = new();


	public async Task<IActionResult> OnGetAsync(string userId)
	{
		try
		{
			if (string.IsNullOrEmpty(userId))
			{
				NotFound();
			}

			var currentUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);

			if (currentUser == null)
			{
				return BadRequest();
			}

			try
			{
				// TODO: Shady - fix issue #903 so this works
				var transactions = await _issuingHandlerTransactionListService.ListByEmailAsync(currentUser.Email);
				Transactions = _mapper.Map<List<StripeTransactionDto>>(transactions);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to load stripe transactions for ", currentUser.Email);
			}

			var roles = await _roleManager.Roles.ToListAsync();

			var unassignedRoles = new List<IdentityRole>();
			var assignedRoles = new List<IdentityRole>();
			// TODO: Fix this awful performing code
			foreach (var role in roles)
			{
				if (!(await _userManager.GetUsersInRoleAsync(role.Name)).Contains(currentUser))
				{
					unassignedRoles.Add(role);
				}
				else
				{
					assignedRoles.Add(role);
				}
			}

			IdentityUser = currentUser;
			RolesNotAssignedToUser = unassignedRoles.Select(x => new SelectListItem(x.Name, x.Id)).ToList();
			Roles = assignedRoles.ToList();

			var memberByUserSpec = new MemberByUserIdSpec(userId);
			var member = await _memberRepository.FirstOrDefaultAsync(memberByUserSpec);
			if (member != null)
			{
				UserPersonalUpdateModel = new UserPersonalUpdateModel(member);
				UserLinksUpdateModel = new UserLinksUpdateModel(member);

				MemberSubscriptionPlans = await _subscriptionPlanRepository.ListAsync();
				_logger.LogInformation($"MemberSubscriptionPlans Count: {MemberSubscriptionPlans.Count}");

				var subscriptionByMemberSpec = new MemberSubscriptionsByMemberSpec(member.Id);
				var subscriptions = await _subscriptionRepository.ListAsync(subscriptionByMemberSpec);

				foreach (var subscription in subscriptions)
				{
					Subscriptions.Add(new SubscriptionDTO()
					{
						Id = subscription.Id,
						StartDate = subscription.Dates.StartDate,
						EndDate = subscription.Dates.EndDate,
						MemberSubscriptionPlan =
							MemberSubscriptionPlans.FirstOrDefault(msp => msp.Id == subscription.MemberSubscriptionPlanId)
					});

					var totalDaysInSubscription = subscription.Dates.EndDate != null
						? ((DateTime)subscription.Dates.EndDate - subscription.Dates.StartDate).TotalDays
						: (DateTime.Today - subscription.Dates.StartDate).TotalDays;
					TotalDaysInAllSubscriptions += totalDaysInSubscription;
				}
			}


			EmailConfirmation.IsConfirmedString = IdentityUser.EmailConfirmed ? "Yes" : "No";
			string emailAddressMessage = "the email address";
			EmailConfirmation.EditEmailConfirmationMessage = @$"Are you sure you want to {(IdentityUser.EmailConfirmed
				? $"revoke {emailAddressMessage} confirmation"
				: $"confirm {emailAddressMessage}")}?";
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "exception");
		}
		

		return Page();
	}

	public async Task<IActionResult> OnPostAddUserToRoleAsync(string userId, string roleId)
	{
		await _userRoleMembershipService.AddUserToRoleAsync(userId, roleId);

		return RedirectToPage("./User", new { userId = userId });
	}

	public async Task<IActionResult> OnPostRemoveUserFromRole(string userId, string roleId)
	{
		await _userRoleMembershipService.RemoveUserFromRoleAsync(userId, roleId);

		return RedirectToPage("./User", new { userId = userId });
	}

	public async Task<IActionResult> OnPostAddSubscriptionAsync(string userId, SubscriptionDTO subscription)
	{
		var memberByUserSpec = new MemberByUserIdSpec(userId);
		var member = await _memberRepository.FirstOrDefaultAsync(memberByUserSpec);

		if (member == null)
		{
			member = await _memberRegistrationService.RegisterMemberAsync(userId);
		}

		var subscriptionByMemberSpec = new MemberSubscriptionsByMemberSpec(member.Id);
		var subscriptionsFromDb = await _subscriptionRepository.ListAsync(subscriptionByMemberSpec);

		// return error message if new subscription overlaps an existing subscription
		var isError = subscriptionsFromDb.Any(x =>
			((subscription.StartDate >= x.Dates.StartDate && subscription.StartDate <= x.Dates.EndDate) ||
			 (subscription.EndDate >= x.Dates.StartDate && subscription.EndDate <= x.Dates.EndDate)));

		if (isError)
		{
			ModelState.AddModelError("OverlappingSubscription", "Subscriptions cannot overlap");
			return BadRequest(ModelState);
		}

		try
		{
			int subscriptionPlanId = 1; // monthly
			if (subscription.MemberSubscriptionPlan != null)
			{
				subscriptionPlanId = subscription.MemberSubscriptionPlan.Id;
			}
			var newSub = new MemberSubscription(member.Id, subscriptionPlanId, new DateTimeRange(subscription.StartDate, subscription.EndDate));

			await _subscriptionRepository.AddAsync(newSub);
		}

		//DateTimeRange throws an error if EndDate is prior to StartDate
		catch (ArgumentException e)
		{
			ModelState.AddModelError("InvalidSubscription", e.Message);
			return BadRequest(ModelState);
		}

		return RedirectToPage("./User", new { userId = userId });
	}

	public async Task<IActionResult> OnPostDeleteSubscriptionAsync(string userId, int subscriptionId)
	{
		var subscriptionEntity = await _subscriptionRepository.GetByIdAsync(subscriptionId);
		if (subscriptionEntity is null) throw new MemberSubscriptionNotFoundException(subscriptionId);
		await _subscriptionRepository.DeleteAsync(subscriptionEntity);

		return RedirectToPage("./User", new { userId = userId });
	}

	public async Task<IActionResult> OnPostEditSubscriptionAsync(string userId, int subscriptionId, SubscriptionDTO subscription)
	{
		var subscriptionEntity = await _subscriptionRepository.GetByIdAsync(subscriptionId);
		if (subscriptionEntity is null) throw new MemberSubscriptionNotFoundException(subscriptionId);
		subscriptionEntity.Dates = new DateTimeRange(subscription.StartDate, subscription.EndDate);
		await _subscriptionRepository.UpdateAsync(subscriptionEntity);

		return RedirectToPage("./User", new { userId = userId });
	}

	public async Task<IActionResult> OnPostUpdateEmailConfirmationAsync(string userId, bool isEmailConfirmed)
	{
		await _userEmailConfirmationService.UpdateUserEmailConfirmationAsync(userId, !isEmailConfirmed);

		return RedirectToPage("./User", new { userId });
	}

	public async Task<IActionResult> OnPostUpdatePersonalInfoAsync(string userId)
	{
		if (!ModelState.IsValid)
		{
			ModelState.AddModelError("InvalidUserId", "Bad Data");
			return BadRequest(ModelState);
		}

		var spec = new MemberByUserIdSpec(userId);
		var member = await _memberRepository.FirstOrDefaultAsync(spec);
		if (member is null) throw new MemberNotFoundException(userId);

		member.UpdateName(UserPersonalUpdateModel.FirstName, UserPersonalUpdateModel.LastName, false);
		member.UpdatePEInfo(UserPersonalUpdateModel.PEFriendCode, UserPersonalUpdateModel.PEUsername, false);
		member.UpdateAboutInfo(UserPersonalUpdateModel.AboutInfo, false);
		member.UpdateAddress(UserPersonalUpdateModel.Address, false);
		member.UpdateDiscord(UserPersonalUpdateModel.DiscordUsername, false);
		member.UpdateEmail(UserPersonalUpdateModel.Email, false);

		await _memberRepository.UpdateAsync(member);

		if (!string.IsNullOrEmpty(UserPersonalUpdateModel.Email))
		{
			var user = await _userManager.FindByIdAsync(userId);
			user.Email = UserPersonalUpdateModel.Email;
			await _userManager.UpdateAsync(user);
		}

		return RedirectToPage("./User", new { userId });
	}
	public async Task<IActionResult> OnPostUpdateLinksAsync(string userId)
	{
		var spec = new MemberByUserIdSpec(userId);
		var member = await _memberRepository.FirstOrDefaultAsync(spec);
		if (member is null) throw new MemberNotFoundException(userId);

		member.UpdateLinks(UserLinksUpdateModel.BlogUrl, UserLinksUpdateModel.CodinGameUrl, UserLinksUpdateModel.GithubUrl, UserLinksUpdateModel.LinkedInUrl,
		UserLinksUpdateModel.OtherUrl, UserLinksUpdateModel.TwitchUrl, UserLinksUpdateModel.YouTubeUrl, UserLinksUpdateModel.TwitterUrl);

		await _memberRepository.UpdateAsync(member);

		return RedirectToPage("./User", new { userId });
	}

	public class SubscriptionDTO
	{
		public int Id { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public MemberSubscriptionPlan? MemberSubscriptionPlan { get; set; }
	}

	public class EmailConfirmationModel
	{
		public string IsConfirmedString { get; set; } = "";
		public string EditEmailConfirmationMessage { get; set; } = "";
	}
}
