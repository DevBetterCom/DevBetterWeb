using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class NewMemberRegisterModel : PageModel
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly RoleManager<IdentityRole> _roleManager;
  private readonly IUserRoleMembershipService _userRoleMembershipService;
  private readonly ILogger<RegisterModel> _logger;
  private readonly IDomainEventDispatcher _dispatcher;
  private readonly ICaptchaValidator _captchaValidator;
  private readonly INewMemberService _newMemberService;

  public NewMemberRegisterModel(
          UserManager<ApplicationUser> userManager,
          RoleManager<IdentityRole> roleManager,
          IUserRoleMembershipService userRoleMembershipService,
          ILogger<RegisterModel> logger,
          IDomainEventDispatcher dispatcher,
          ICaptchaValidator captchaValidator,
          INewMemberService newMemberService)
  {
    _userManager = userManager;
    _roleManager = roleManager;
    _userRoleMembershipService = userRoleMembershipService;
    _logger = logger;
    _dispatcher = dispatcher;
    _captchaValidator = captchaValidator;
    _newMemberService = newMemberService;
  }

  [BindProperty]
  public InputModel? Input { get; set; }

  public string ReturnUrl { get; set; } = "../User/MyProfile/Personal";
  public string? ErrorMessage { get; set; }
  public string? Email { get; set; }

	public class InputModel
  {
    [Required]
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }
    [Required]
    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [Required]
    [Display(Name = "Country")]
    public string? Country { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
		[Display(Name = "City")]
    public string? City { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[Display(Name = "Street Address")]
    public string? StreetAddress { get; set; }

    [Required]
    [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
		[Display(Name = "Postal Code")]
    public string? PostalCode { get; set; }

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

    return Page();
  }

  public PageResult DisplayErrorMessage(string message)
  {
    ErrorMessage = message;
    return Page();
  }

  public async Task<IActionResult> OnPostAsync(string captcha, string inviteCode, string email, string? returnUrl = null)
  {
	  try
	  {
		  returnUrl ??= Url.Content("~/");
		  if (!await _captchaValidator.IsCaptchaPassedAsync(captcha))
		  {
			  ModelState.AddModelError("captcha", "Captcha validation failed");
		  }

		  if (ModelState.IsValid)
		  {
			  if (Input is null) throw new Exception("Input is null.");
			  var user = new ApplicationUser { UserName = email, Email = email, DateCreated = DateTime.UtcNow };
			  var result = await _userManager.CreateAsync(user, Input!.Password!);
			  if (result.Succeeded)
			  {
				  _logger.LogInformation("User created a new account with password.");

				  var newUserEvent = new NewUserRegisteredEvent(email,
					  Request.HttpContext.Connection.RemoteIpAddress!.ToString());

				  await _dispatcher.Dispatch(newUserEvent);

				  var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
				  var userId = user.Id;

				  var emailConfirmationResult = await _userManager.ConfirmEmailAsync(user, code);
				  if (!emailConfirmationResult.Succeeded)
				  {
					  throw new InvalidOperationException($"Error confirming email for user with ID '{userId}':");
				  }

				  await _newMemberService.MemberSetupAsync(userId, Input.FirstName!, Input.LastName!, inviteCode!, email, Input.StreetAddress!, Input.City!, Input.Country!, Input.PostalCode!);

				  _logger.LogInformation($"Adding user {user.Email} to Member Role");
				  var roles = await _roleManager.Roles.ToListAsync();
				  var memberRole = roles.FirstOrDefault(r => r.Name == "Member");
				  if (memberRole != null)
				  {
					  await _userRoleMembershipService.AddUserToRoleAsync(userId, memberRole.Id);
				  }

				  return Redirect("/User/MyProfile");
			  }

			  foreach (var error in result.Errors)
			  {
				  ModelState.AddModelError(string.Empty, error.Description);
			  }
		  }
	  }
	  catch (Exception exception)
	  {
			_logger.LogError(exception, "NewUserRegistered Exception");
			var exceptionEvent = new ExceptionEvent(exception);
			await _dispatcher.Dispatch(exceptionEvent);
	  }
    

    // If we got this far, something failed, redisplay form
    return Page();
  }
}
