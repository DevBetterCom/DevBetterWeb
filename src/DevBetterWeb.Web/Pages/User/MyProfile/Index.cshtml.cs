using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http;
using System.Net;
using System.Globalization;
using System.Text;
using System.Security.Cryptography;

namespace DevBetterWeb.Web.Pages.User.MyProfile;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
public class IndexModel : PageModel
{
#nullable disable
	[BindProperty]
	public UserProfileViewModel UserProfileViewModel { get; set; }
	public string AvatarUrl { get; set; }

	public List<Book> Books { get; set; } = new List<Book>();

	public string AlumniProgressPercentage { get; set; }
	public MemberSubscriptionPercentBarViewModel Model { get; set; }

#nullable enable

	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IMemberRegistrationService _memberRegistrationService;
	private readonly IRepository<Member> _memberRepository;
	private readonly IRepository<Book> _bookRepository;
	private readonly IMemberSubscriptionPeriodCalculationsService _memberSubscriptionPeriodCalculationsService;
	private readonly IOptions<TwitterSettings> _twitterSettings;

	public IndexModel(UserManager<ApplicationUser> userManager,
			IMemberRegistrationService memberRegistrationService,
			IRepository<Member> memberRepository,
			IRepository<Book> bookRepository,
			IMemberSubscriptionPeriodCalculationsService memberSubscriptionPeriodCalculationsService,
			IOptions<TwitterSettings> apiSettings)
	{
		_userManager = userManager;
		_memberRegistrationService = memberRegistrationService;
		_memberRepository = memberRepository;
		_bookRepository = bookRepository;
		_memberSubscriptionPeriodCalculationsService = memberSubscriptionPeriodCalculationsService;
		_twitterSettings = apiSettings;
	}

	public async Task OnGetAsync()
	{
		var currentUserName = User.Identity!.Name;
		var applicationUser = await _userManager.FindByNameAsync(currentUserName);
		AvatarUrl = string.Format(Constants.AVATAR_IMGURL_FORMAT_STRING, applicationUser.Id);

		var spec = new MemberByUserIdWithBooksReadAndMemberSubscriptionsSpec(applicationUser.Id);
		var member = await _memberRepository.FirstOrDefaultAsync(spec);

		if (member == null)
		{
			member = await _memberRegistrationService.RegisterMemberAsync(applicationUser.Id);
		}

		Books = await _bookRepository.ListAsync();

		int percentage = _memberSubscriptionPeriodCalculationsService.GetPercentageProgressToAlumniStatus(member);

		Model = new MemberSubscriptionPercentBarViewModel(percentage);

		UserProfileViewModel = new UserProfileViewModel(member);
	}

	public bool GetIsAlumni()
	{
		return User.IsInRole(AuthConstants.Roles.ALUMNI);
	}

	public async Task<ActionResult> OnPostTwitterAsync()
	{
		// Begin Twitter Auth flow
	}

	public async Task<ActionResult> OnGetTwitterAsync()
	{
		// Finish Twitter Auth flow
		// Save user credentials to database
		return RedirectToPage();
	}
}	
