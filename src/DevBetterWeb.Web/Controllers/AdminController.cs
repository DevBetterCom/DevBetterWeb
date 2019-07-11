﻿using System;
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

namespace DevBetterWeb.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IdentityDbContext _identityDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(IdentityDbContext identityDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _identityDbContext = identityDbContext;
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

        public IActionResult User(string userId)
        {
            var userViewModel = new UserViewModel();

            userViewModel.User = _identityDbContext.Users.FirstOrDefault(x => x.Id == userId);
            return View(userViewModel);
        }

        public async Task<IActionResult> Role(string roleId)
        {
            var role = _roleManager.Roles.FirstOrDefault(x => x.Id == roleId);

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

            var userIdsInRole = usersInRole.Select(X => X.Id).ToList();
            
            //var userIdsOfUsersInRole = _identityDbContext.UserRoles.Where(x => x.RoleId == role.Id).Select(x => x.UserId).ToList();
            //var usersInRole = _identityDbContext.Users.Where(x => userIdsOfUsersInRole.Contains(x.Id)).ToList();

            var usersNotInRole = _userManager.Users.Where(x => !userIdsInRole.Contains(x.Id)).ToList();

            var roleViewModel = new RoleViewModel();
            roleViewModel.Role = role;
            roleViewModel.UsersInRole = usersInRole.ToList();
            roleViewModel.UsersNotInRole = usersNotInRole.Select(x => new SelectListItem(x.Email, x.Id)).ToList();


            return View(roleViewModel);
        }

        [HttpPost]
        public IActionResult AddUserToRole(string userId, string roleId)
        {
            var user = _identityDbContext.Users.FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                return BadRequest();
            }


            return RedirectToAction("role", new { roleId });

        }
    }
}