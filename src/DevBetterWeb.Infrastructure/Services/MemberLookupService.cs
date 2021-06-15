using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;

namespace DevBetterWeb.Infrastructure.Services
{
  public class MemberLookupService : IMemberLookupService
  {
    private readonly IUserLookupService _userLookup;
    private readonly IRepository<Member> _memberRepository;

    public MemberLookupService(IUserLookupService userLookupService,
      IRepository<Member> repository)
    {
      _userLookup = userLookupService;
      _memberRepository = repository;
    }

    public async Task<Member> GetMemberByEmailAsync(string memberEmail)
    {
      var userId = await _userLookup.FindUserIdByEmailAsync(memberEmail);

      var spec = new MemberByUserIdSpec(userId);
      var member = await _memberRepository.GetBySpecAsync(spec);
      if (member is null) throw new MemberWithEmailNotFoundException(memberEmail);

      return member;
    }
  }
}
