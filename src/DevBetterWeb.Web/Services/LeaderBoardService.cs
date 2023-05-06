using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;

namespace DevBetterWeb.Web.Services;

public class LeaderBoardService
{
	private readonly IRepository<Member> _memberRepository;

	public LeaderBoardService(IRepository<Member> memberRepository)
	{
		_memberRepository = memberRepository;
	}

	private Task<List<Member>> GetNonCurrentMembers(CancellationToken cancellationToken = default)
	{
		var membersNonSubscriptionSpec = new MembersNonSubscriptionSpec();
		var nonMembers = _memberRepository.ListAsync(membersNonSubscriptionSpec, cancellationToken);

		return nonMembers;
	}
}
