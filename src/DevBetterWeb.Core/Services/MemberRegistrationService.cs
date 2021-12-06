using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;

namespace DevBetterWeb.Core.Services;

public class MemberRegistrationService : IMemberRegistrationService
{
  private readonly IRepository<Member> _memberRepository;

  public MemberRegistrationService(IRepository<Member> memberRepository)
  {
    _memberRepository = memberRepository;
  }

  public async Task<Member> RegisterMemberAsync(string userId)
  {
    var member = new Member(userId);

    await _memberRepository.AddAsync(member);

    return member;
  }
}
