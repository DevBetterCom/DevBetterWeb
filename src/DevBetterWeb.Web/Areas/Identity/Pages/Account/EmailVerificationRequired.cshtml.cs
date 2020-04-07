using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Areas.Identity.Pages.Account
{
    // [Authorize] This page should allow anonymous otherwise un verified users can't see it.
    public class EmailVerificationRequiredModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailVerificationRequiredModel> _logger;
        public bool VerificationEmailSent { get; set; }

        public EmailVerificationRequiredModel(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            ILogger<EmailVerificationRequiredModel> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task OnPostAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = user.Id, code },
                protocol: Request.Scheme);

            await _emailService.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }
    }
}
