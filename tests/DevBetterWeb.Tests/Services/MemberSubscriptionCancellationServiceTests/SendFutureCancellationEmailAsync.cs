using System;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Specs;
using NSubstitute;
using Xunit;

namespace DevBetterWeb.Tests.Services.MemberSubscriptionCancellationServiceTests;

public class SendFutureCancellationEmailAsync : MemberSubscriptionCancellationServiceTest
{
	private readonly string _email = "TestEmail";
	private readonly DateTime _date = new DateTime(1, 1, 1);

	[Fact]
	public async Task SendsEmail()
	{
		_memberRepository.FirstOrDefaultAsync(
				Arg.Any<MemberByUserIdSpec>(),
				CancellationToken.None)!
			.Returns(Task.FromResult(new Member()));

		_subscriptionPeriodCalculationsService.GetCurrentSubscriptionEndDate(
				Arg.Any<Member>())
			.Returns(_date);

		await _memberCancellationService.SendFutureCancellationEmailAsync(_email);

		await _emailService.Received()
			.SendEmailAsync(_email, Arg.Any<string>(), Arg.Any<string>());
	}
}
