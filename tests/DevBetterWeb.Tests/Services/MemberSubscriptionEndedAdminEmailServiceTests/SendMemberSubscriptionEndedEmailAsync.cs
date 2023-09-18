using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Xunit;

namespace DevBetterWeb.Tests.Services.MemberSubscriptionEndedAdminEmailServiceTests;

public class SendMemberSubscriptionEndedEmailAsync
{
	private readonly MemberSubscriptionEndedAdminEmailService _memberSubscriptionEndedAdminEmailService;

	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IEmailService _emailService;
	private readonly IMemberLookupService _memberLookup;

	public SendMemberSubscriptionEndedEmailAsync()
	{
		var store = Substitute.For<IUserStore<ApplicationUser>>();
		_userManager = Substitute.For<UserManager<ApplicationUser>>(store, null, null, null, null, null, null, null, null);
		_emailService = Substitute.For<IEmailService>();
		_memberLookup = Substitute.For<IMemberLookupService>();
		_memberSubscriptionEndedAdminEmailService = new MemberSubscriptionEndedAdminEmailService(
			_userManager, _emailService, _memberLookup);
	}

	[Fact]
	public async Task SendsEmail()
	{
		_memberLookup.GetMemberByEmailAsync(Arg.Any<string>())
			.Returns(Task.FromResult(Substitute.For<Member>()));
		_userManager.GetUsersInRoleAsync(AuthConstants.Roles.ADMINISTRATORS)
			.Returns(Task.FromResult<IList<ApplicationUser>>(new List<ApplicationUser> { Substitute.For<ApplicationUser>(), Substitute.For<ApplicationUser>() }));

		string testEmail = "TestEmail";
		await _memberSubscriptionEndedAdminEmailService.SendMemberSubscriptionEndedEmailAsync(testEmail, null);

		await _emailService.Received(2)
			.SendEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
	}
}
