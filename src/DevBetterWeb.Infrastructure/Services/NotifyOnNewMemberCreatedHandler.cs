using DevBetterWeb.Core;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.Infrastructure.Services
{
    public class NotifyOnNewMemberCreatedHandler : IHandle<NewMemberCreatedEvent>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public NotifyOnNewMemberCreatedHandler(UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }
        public async Task Handle(NewMemberCreatedEvent domainEvent)
        {
            var usersInAdminRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.ADMINISTRATORS);

            foreach(var emailAddress in usersInAdminRole.Select(user => user.Email))
            {
                string subject = $"[devBetter] New Member {domainEvent.Member.UserFullName()}";
                string message = $"A new Member with id {domainEvent.Member.UserId} has signed up and added their membership profile."; 
                await _emailService.SendEmailAsync(emailAddress, subject, message);
            }
        }
    }
}
