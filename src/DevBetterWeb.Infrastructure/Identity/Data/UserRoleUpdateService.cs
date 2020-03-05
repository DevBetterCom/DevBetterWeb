using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Models
{
    public class UserRoleUpdateService : IUserRoleUpdateService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public UserRoleUpdateService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IDomainEventDispatcher domainEventDispatcher)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _domainEventDispatcher = domainEventDispatcher;
        }

        public async Task AddUserToRoleAsync(string userId, string roleId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == roleId);

            if (user == null || role == null)
            {
                // TODO: Use Result<T> library
                throw new System.Exception("User and/or Role not found.");
            }

            await _userManager.AddToRoleAsync(user, role.Name);

            var domainEvent = new UserAddedToRoleEvent(userId, role.Name);
            _domainEventDispatcher.Dispatch(domainEvent);
        }

        public async Task RemoveUserFromRoleAsync(string userId, string roleId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == roleId);

            if (user == null || role == null)
            {
                // TODO: Use Result<T> library
                throw new System.Exception("User and/or Role not found.");
            }

            await _userManager.RemoveFromRoleAsync(user, role.Name);
        }
    }
}
