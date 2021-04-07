using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
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

    [BindProperty]
    public string Email { get; set; } = "";

    public async Task OnPostAsync()
    {
      VerificationEmailSent = true;

      string email = User?.Identity?.Name ?? Email;
      var user = await _userManager.FindByNameAsync(email);
      if (user is null) return;
      var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
      var callbackUrl = Url.Page(
          "/Account/ConfirmEmail",
          pageHandler: null,
          values: new { userId = user.Id, code },
          protocol: Request.Scheme);

      _logger.LogInformation($"Sending email to {email} with verification link.");
      await _emailService.SendEmailAsync(email, "Confirm your email",
          $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
    }
  }
}
