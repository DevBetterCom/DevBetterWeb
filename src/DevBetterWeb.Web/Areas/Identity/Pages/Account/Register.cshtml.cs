using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using GoogleReCaptcha.V3.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class RegisterModel : PageModel
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly ILogger<RegisterModel> _logger;
  private readonly IEmailService _emailService;
  private readonly ICaptchaValidator _captchaValidator;
	private readonly IMediator _mediator;

	public RegisterModel(
          UserManager<ApplicationUser> userManager,
          ILogger<RegisterModel> logger,
          IEmailService emailService,
          ICaptchaValidator captchaValidator,
					IMediator mediator)
  {
    _userManager = userManager;
    _logger = logger;
    _emailService = emailService;
    _captchaValidator = captchaValidator;
		_mediator = mediator;
	}

  [BindProperty]
  public InputModel? Input { get; set; }

  public string? ReturnUrl { get; set; }

  public class InputModel
  {
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string? Email { get; set; }

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

  public void OnGet(string? returnUrl = null)
  {
    ReturnUrl = returnUrl;
  }

  public async Task<IActionResult> OnPostAsync(string captcha, string? returnUrl = null)
  {
	  try
	  {
		  returnUrl = returnUrl ?? Url.Content("~/");
	    if (!await _captchaValidator.IsCaptchaPassedAsync(captcha))
	    {
	      ModelState.AddModelError("captcha", "Captcha validation failed");
	    }
	    if (ModelState.IsValid)
	    {
	      if (Input is null) throw new Exception("Input is null.");
	      var user = new ApplicationUser { UserName = Input.Email!, Email = Input.Email!, DateCreated = DateTime.UtcNow };
	      var result = await _userManager.CreateAsync(user, Input.Password!);
	      if (result.Succeeded)
	      {
	        _logger.LogInformation("User created a new account with password.");

	        var newUserEvent = new NewUserRegisteredEvent(Input.Email!,
	          Request.HttpContext.Connection.RemoteIpAddress!.ToString());

	        await _mediator.Publish(newUserEvent);

	        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
	        var callbackUrl = Url.Page(
	            "/Account/ConfirmEmail",
	            pageHandler: null,
	            values: new { userId = user.Id, code = code },
	            protocol: Request.Scheme);
	        if (string.IsNullOrEmpty(callbackUrl)) throw new Exception("Callback URL is null or empty.");

	        if (string.IsNullOrEmpty(Input.Email)) throw new Exception("Email is required.");
	        await _emailService.SendEmailAsync(Input.Email, "Confirm your email",
	            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

	        return LocalRedirect("~/Identity/Account/EmailVerificationRequired");
	      }
	      foreach (var error in result.Errors)
	      {
	        ModelState.AddModelError(string.Empty, error.Description);
	      }
	    }
	  }
	  catch (Exception exception)
	  {
		  _logger.LogError(exception, "RegisterModel Exception");
		  var exceptionEvent = new ExceptionEvent(exception);
		  await _mediator.Publish(exceptionEvent);
	  }
		// If we got this far, something failed, redisplay form
		return Page();
  }
}
