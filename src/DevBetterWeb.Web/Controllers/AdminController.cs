using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using DevBetterWeb.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using DevBetterWeb.Core;

namespace DevBetterWeb.Web.Controllers
{
    //[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
           _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var viewModel = new AdminIndexViewModel();
            viewModel.Users = _userManager.Users.ToList();
            viewModel.Roles = _roleManager.Roles.ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> User(string userId)
        {
            var currentUser = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            var roles = _roleManager.Roles;

            var unassignedRoles = new List<IdentityRole>();
            var assignedRoles = new List<IdentityRole>();
            foreach (var role in roles)
            {
                if(! (await _userManager.GetUsersInRoleAsync(role.Name)).Contains(currentUser))
                {
                    unassignedRoles.Add(role);
                }
                else
                {
                    assignedRoles.Add(role);
                }
            }

            var userViewModel = new UserViewModel();
            userViewModel.User = currentUser;
            userViewModel.RolesNotAssignedToUser = unassignedRoles.Select(x => new SelectListItem(x.Name, x.Id)).ToList();
            userViewModel.Roles = assignedRoles.ToList();
            return View(userViewModel);
        }

        public async Task<IActionResult> Role(string roleId)
        {
            var role = _roleManager.Roles.Single(x => x.Id == roleId);

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

            var userIdsInRole = usersInRole.Select(X => X.Id).ToList();
            var usersNotInRole = _userManager.Users.Where(x => !userIdsInRole.Contains(x.Id)).ToList();

            var roleViewModel = new RoleViewModel();
            roleViewModel.Role = role;
            roleViewModel.UsersInRole = usersInRole.ToList();
            roleViewModel.UsersNotInRole = usersNotInRole.Select(x => new SelectListItem(x.Email, x.Id)).ToList();


            return View(roleViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToRole(string userId, string roleId)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            var role = _roleManager.Roles.FirstOrDefault(x => x.Id == roleId);

            if (user == null || role == null)
            {
                return BadRequest();
            }

            await _userManager.AddToRoleAsync(user, role.Name);

            return RedirectToAction("index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUserFromRole(string userId, string roleId)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            var role = _roleManager.Roles.FirstOrDefault(x => x.Id == roleId);

            if (user == null || role == null)
            {
                return BadRequest();
            }

            await _userManager.RemoveFromRoleAsync(user, role.Name);

            return RedirectToAction("index");
        }
    }
}