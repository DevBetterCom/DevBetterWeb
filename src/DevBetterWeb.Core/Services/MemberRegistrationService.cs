using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Services
{
  public class MemberRegistrationService : IMemberRegistrationService
  {
    private readonly IRepository _repository;

    public MemberRegistrationService(IRepository repository)
    {
      _repository = repository;
    }

    public async Task<Member> RegisterMemberAsync(string userId)
    {
      var member = new Member(userId);

      await _repository.AddAsync(member);

      return member;
    }
  }
}
