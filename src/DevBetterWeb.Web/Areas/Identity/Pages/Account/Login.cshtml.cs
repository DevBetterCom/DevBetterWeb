using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
  private readonly SignInManager<ApplicationUser> _signInManager;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IEmailService _emailService;
  private readonly ILogger<LoginModel> _logger;

  public LoginModel(SignInManager<ApplicationUser> signInManager,
      UserManager<ApplicationUser> userManager,
      IEmailService emailService,
      ILogger<LoginModel> logger)
  {
    _signInManager = signInManager;
    _userManager = userManager;
    _emailService = emailService;
    _logger = logger;
  }

#nullable disable
  [BindProperty]
  public InputModel Input { get; set; }
#nullable enable

  public IList<AuthenticationScheme>? ExternalLogins { get; set; }

  public string? ReturnUrl { get; set; }

  [TempData]
  public string? ErrorMessage { get; set; }

  public class InputModel
  {
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
  }

  public async Task OnGetAsync(string? returnUrl = null)
  {
    if (!string.IsNullOrEmpty(ErrorMessage))
    {
      ModelState.AddModelError(string.Empty, ErrorMessage);
    }

    returnUrl = returnUrl ?? Url.Content("~/");

    // Clear the existing external cookie to ensure a clean login process
    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

    ReturnUrl = returnUrl;
  }

  public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
  {
    returnUrl = returnUrl ?? Url.Content("/User/MyProfile/Index");

    if (ModelState.IsValid)
    {
      // This doesn't count login failures towards account lockout
      // To enable password failures to trigger account lockout, set lockoutOnFailure: true
      var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
      if (result.Succeeded)
      {
				_logger.LogInformation($"LOGIN SUCCESS for {Input.Email}!");
        return LocalRedirect(returnUrl);
      }
      if (result.IsNotAllowed)
      {
        var user = _userManager.Users.First(x => x.Email == Input.Email);
        if (!user.EmailConfirmed)
        {
          _logger.LogInformation($"User {user.Email} logged in, but email has not been confirmed.");

          var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
          var callbackUrl = Url.Page(
              "/Account/ConfirmEmail",
              pageHandler: null,
              values: new { userId = user.Id, code },
              protocol: Request.Scheme);
          if (string.IsNullOrEmpty(callbackUrl)) throw new Exception("Callback URL is null or empty.");

          if (Input.Email == null) throw new Exception("Email is required.");
          await _emailService.SendEmailAsync(Input.Email, "Confirm your email",
              $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

          //await _signInManager.SignInAsync(user, isPersistent: false);
          return LocalRedirect("~/Identity/Account/EmailVerificationRequired");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return Page();
      }

      if (result.RequiresTwoFactor)
      {
        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
      }
      if (result.IsLockedOut)
      {
        _logger.LogWarning($"User account {Input.Email} locked out.");
        return RedirectToPage("./Lockout");
      }
      else
      {
        ModelState.AddModelError(string.Empty, $"Invalid login attempt by user {Input.Email}.");
        return Page();
      }
    }

    // If we got this far, something failed, redisplay form
    return Page();
  }
}
