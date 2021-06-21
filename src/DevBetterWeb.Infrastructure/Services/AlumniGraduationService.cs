using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;

namespace DevBetterWeb.Infrastructure.Services
{
  public class AlumniGraduationService : IAlumniGraduationService
  {
    private const int DAYS_IN_TWO_YEARS = 365 * 2;

    private readonly IUserLookupService _userLookupService;

    public AlumniGraduationService(IUserLookupService userLookupService)
    {
      _userLookupService = userLookupService;
    }

    public async Task<List<Member>> CheckIfAnyMemberGraduating(List<Member> members)
    {
      var list = new List<Member>();

      foreach(var member in members)
      {
        if(member.TotalSubscribedDays() >= DAYS_IN_TWO_YEARS)
        {
          bool memberIsAlumnus = await _userLookupService.FindUserIsAlumniByUserIdAsync(member.UserId);

          if(!memberIsAlumnus)
          {
            list.Add(member);
          }
        }
      }

      return list;
    }

    public Task GraduateMembersIfNeeded(AppendOnlyStringList messages)
    {
      throw new System.NotImplementedException();
    }
  }
}
