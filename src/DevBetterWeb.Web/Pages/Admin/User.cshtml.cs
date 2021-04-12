using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Core.ValueObjects;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.Admin
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
  public class UserModel : PageModel
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserRoleMembershipService _userRoleMembershipService;
    private readonly IMemberRegistrationService _memberRegistrationService;
    private readonly IRepository _repository;

    public UserModel(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IUserRoleMembershipService userRoleMembershipService,
        IMemberRegistrationService memberRegistrationService,
        IRepository repository)
    {
      _userManager = userManager;
      _roleManager = roleManager;
      _userRoleMembershipService = userRoleMembershipService;
      _memberRegistrationService = memberRegistrationService;
      _repository = repository;
    }


    public IdentityUser? IdentityUser { get; set; }
    public List<IdentityRole> Roles { get; set; } = new List<IdentityRole>();
    public List<SelectListItem> RolesNotAssignedToUser { get; set; } = new List<SelectListItem>();
    public SubscriptionDTO Subscription { get; set; } = new SubscriptionDTO();
    public List<SubscriptionDTO> Subscriptions { get; set; } = new List<SubscriptionDTO>();
    public double TotalDaysInAllSubscriptions { get; set; }


    public async Task<IActionResult> OnGetAsync(string userId)
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

      var roles = await _roleManager.Roles.ToListAsync();

      var unassignedRoles = new List<IdentityRole>();
      var assignedRoles = new List<IdentityRole>();
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
      var member = await _repository.GetAsync(memberByUserSpec);
      var subscriptions = new List<MemberSubscription>();
      if (member != null)
      {
        var subscriptionByMemberSpec = new SubscriptionsByMemberSpec(member.Id);
        subscriptions = await _repository.ListAsync(subscriptionByMemberSpec);

        foreach (var subscription in subscriptions)
        {
          Subscriptions.Add(new SubscriptionDTO()
          {
            Id = subscription.Id,
            StartDate = subscription.Dates.StartDate,
            EndDate = subscription.Dates.EndDate,
          });

          var totalDaysInSubscription = subscription.Dates.EndDate != null ? ((DateTime)subscription.Dates.EndDate - subscription.Dates.StartDate).TotalDays : (DateTime.Today - subscription.Dates.StartDate).TotalDays;
          TotalDaysInAllSubscriptions += totalDaysInSubscription;
        }
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
      var member = await _repository.GetAsync(memberByUserSpec);

      if (member == null)
      {
        member = await _memberRegistrationService.RegisterMemberAsync(userId);
      }

      var subscriptionByMemberSpec = new SubscriptionsByMemberSpec(member.Id);
      var subscriptionsFromDb = await _repository.ListAsync(subscriptionByMemberSpec);

      // return error message if new subscription overlaps an existing subscription
      foreach (var subscriptionFromDb in subscriptionsFromDb)
      {
        if ((subscription.StartDate >= subscriptionFromDb.Dates.StartDate && subscription.StartDate <= subscriptionFromDb.Dates.EndDate) ||
            (subscription.EndDate >= subscriptionFromDb.Dates.StartDate && subscription.EndDate <= subscriptionFromDb.Dates.EndDate))
        {
          ModelState.AddModelError("OverlappingSubscription", "Subscriptions cannot overlap");
          return BadRequest(ModelState);
        }
      }

      try
      {
        await _repository.AddAsync(new MemberSubscription() { Dates = new DateTimeRange(subscription.StartDate, subscription.EndDate), MemberId = member.Id });
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
      var subscriptionEntity = await _repository.GetByIdAsync<MemberSubscription>(subscriptionId);
      await _repository.DeleteAsync(subscriptionEntity);

      return RedirectToPage("./User", new { userId = userId });
    }

    public async Task<IActionResult> OnPostEditSubscriptionAsync(string userId, int subscriptionId, SubscriptionDTO subscription)
    {
      var subscriptionEntity = await _repository.GetByIdAsync<MemberSubscription>(subscriptionId);
      subscriptionEntity.Dates = new DateTimeRange(subscription.StartDate, subscription.EndDate);
      await _repository.UpdateAsync(subscriptionEntity);

      return RedirectToPage("./User", new { userId = userId });
    }

    public class SubscriptionDTO
    {
      public int Id { get; set; }
      public DateTime StartDate { get; set; }
      public DateTime? EndDate { get; set; }
    }
  }
}
