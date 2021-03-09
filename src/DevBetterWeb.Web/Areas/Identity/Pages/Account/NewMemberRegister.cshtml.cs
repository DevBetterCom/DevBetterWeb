using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Web.Areas.Identity.Data;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Web.Areas.Identity.Pages.Account
{
  [AllowAnonymous]
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
  public class NewMemberRegisterModel : PageModel
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailService _emailService;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly ICaptchaValidator _captchaValidator;
    private readonly INewMemberService _newMemberService;

    public NewMemberRegisterModel(
            UserManager<ApplicationUser> userManager,
            ILogger<RegisterModel> logger,
            IEmailService emailService,
            IDomainEventDispatcher dispatcher,
            ICaptchaValidator captchaValidator,
            INewMemberService newMemberService)
    {
      _userManager = userManager;
      _logger = logger;
      _emailService = emailService;
      _dispatcher = dispatcher;
      _captchaValidator = captchaValidator;
      _newMemberService = newMemberService;

      ReturnUrl = "../User/MyProfile/Personal";
    }

    [BindProperty]
    public InputModel? Input { get; set; }

    public string ReturnUrl { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Email { get; set; }
    public string? InviteCode { get; set; }

    public class InputModel
    {
      [Required]
      [Display(Name = "First Name")]
      public string? FirstName { get; set; }
      [Required]
      [Display(Name = "Last Name")]
      public string? LastName { get; set; }

      [Required]
      [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
      [DataType(DataType.Password)]
      [Display(Name = "Password")]
      public string? Password { get; set; }

      [DataType(DataType.Password)]
      [Display(Name = "Confirm password")]
      [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
      public string? ConfirmPassword { get; set; }
    }

    public async Task<PageResult> OnGet(string inviteCode, string email)
    {
      ErrorMessage = "";

      var validEmailAndInviteCode = await _newMemberService.VerifyValidEmailAndInviteCodeAsync(email, inviteCode);
      _logger.LogInformation($"{validEmailAndInviteCode.Value}");

      if (!validEmailAndInviteCode.Value.Equals("success"))
      {
        var errorMessage = "Your email or invite code is invalid. Please confirm you've clicked the correct link and try again.";
        DisplayErrorMessage(errorMessage);
      }

      // if we get this far, email and invite code are valid
      Email = email;
      InviteCode = inviteCode;

      return Page();
    }

    public PageResult DisplayErrorMessage(string message)
    {
      ErrorMessage = message;
      return Page();
    }

    public async Task<IActionResult> OnPostAsync(string captcha, string? returnUrl = null)
    {
      returnUrl = returnUrl ?? Url.Content("~/");
      if (!await _captchaValidator.IsCaptchaPassedAsync(captcha))
      {
        ModelState.AddModelError("captcha", "Captcha validation failed");
      }
      if (ModelState.IsValid)
      {
        if (Input is null) throw new Exception("Input is null.");
        var user = new ApplicationUser { UserName = Email, Email = Email };
        var result = await _userManager.CreateAsync(user, Input.Password);
        if (result.Succeeded)
        {
          _logger.LogInformation("User created a new account with password.");

          var newUserEvent = new NewUserRegisteredEvent(Email!,
            Request.HttpContext.Connection.RemoteIpAddress!.ToString());

          await _dispatcher.Dispatch(newUserEvent);

          var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
          var userId = user.Id;

          var emailConfirmationResult = await _userManager.ConfirmEmailAsync(user, code);
          if (!emailConfirmationResult.Succeeded)
          {
            throw new InvalidOperationException($"Error confirming email for user with ID '{userId}':");
          }

          await _newMemberService.MemberSetupAsync(userId, Input.FirstName!, Input.LastName!, InviteCode!);

          // redirect to edit basic profile page
        }
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
      }

      // If we got this far, something failed, redisplay form
      return Page();
    }
  }
}
