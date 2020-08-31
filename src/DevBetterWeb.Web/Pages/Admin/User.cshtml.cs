using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        private readonly IRepository _repository;

        public UserModel(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            IUserRoleMembershipService userRoleMembershipService,
            IRepository repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userRoleMembershipService = userRoleMembershipService;
            _repository = repository;
        }

        public IdentityUser? IdentityUser { get; set; }
        public List<IdentityRole> Roles { get; set; } = new List<IdentityRole>();
        public List<SelectListItem> RolesNotAssignedToUser { get; set; } = new List<SelectListItem>();
        public SubscriptionDTO Subscription { get; set; } = new SubscriptionDTO();
        public List<SubscriptionDTO> Subscriptions { get; set; } = new List<SubscriptionDTO>();


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
                if (! (await _userManager.GetUsersInRoleAsync(role.Name)).Contains(currentUser))
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
            var member = await _repository.GetBySpecAsync(memberByUserSpec);
            var subscriptionByMemberSpec = new SubscriptionsByMemberSpec(member.Id);
            var subscriptions = await _repository.ListBySpecAsync(subscriptionByMemberSpec);

            foreach (var subscription in subscriptions)
            {
                Subscriptions.Add(new SubscriptionDTO()
                {
                    Id = subscription.Id,
                    StartDate = subscription.StartDate,
                    EndDate = subscription.EndDate
                });
            }

            return Page();
        }

        public void OnPostAddSubscription(string userId, SubscriptionDTO subscription)
        {
            
        }

        public async Task<IActionResult> OnPostAddUserToRoleAsync(string userId, string roleId)
        {
            await _userRoleMembershipService.AddUserToRoleAsync(userId, roleId);

            return RedirectToPage("./User", new { userId = userId });
        }

        public async Task<IActionResult> OnPostRemoveUserFromRole(string userId, string roleId)
        {
            await _userRoleMembershipService.RemoveUserFromRoleAsync(userId, roleId);

            return RedirectToPage("./User", new { userId = userId});
        }

        public class SubscriptionDTO
        {
            public int Id { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }
    }
}