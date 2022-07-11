using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Infrastructure.DomainEvents;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ForgotPasswordModel : PageModel
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IEmailSender _emailSender;
  private readonly DomainEventDispatcher _dispatcher;
  private readonly ILogger<ForgotPasswordModel> _logger;

  public ForgotPasswordModel(UserManager<ApplicationUser> userManager,
      IEmailSender emailSender,
      DomainEventDispatcher dispatcher,
      ILogger<ForgotPasswordModel> logger)
  {
    _userManager = userManager;
    _emailSender = emailSender;
    _dispatcher = dispatcher;
    _logger = logger;
  }

  [BindProperty]
  public InputModel? Input { get; set; }

  public class InputModel
  {
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
  }

  public async Task<IActionResult> OnPostAsync()
  {
    if (ModelState.IsValid)
    {
      var user = await _userManager.FindByEmailAsync(Input!.Email);
      if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
      {
        _logger.LogWarning($"User {Input!.Email} does not exist or is not confirmed.");

        var noUserEvent = new InvalidUserEvent(Input!.Email!);
        await _dispatcher.Dispatch(noUserEvent);

        // Don't reveal that the user does not exist or is not confirmed
        return RedirectToPage("./ForgotPasswordConfirmation");
      }

      // For more information on how to enable account confirmation and password reset please 
      // visit https://go.microsoft.com/fwlink/?LinkID=532713
      var code = await _userManager.GeneratePasswordResetTokenAsync(user);
      code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
      var callbackUrl = Url.Page(
          "/Account/ResetPassword",
          pageHandler: null,
          values: new { code },
          protocol: Request.Scheme);
      if (string.IsNullOrEmpty(callbackUrl)) throw new Exception("Callback URL is null or empty.");

      _logger.LogInformation("Sending password reset request with URL " + callbackUrl);

      await _emailSender.SendEmailAsync(
          Input.Email,
          "Reset Password",
          $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

      var newEvent = new PasswordResetEvent(Input.Email!);
      await _dispatcher.Dispatch(newEvent);

      return RedirectToPage("./ForgotPasswordConfirmation");
    }

    return Page();
  }
}
