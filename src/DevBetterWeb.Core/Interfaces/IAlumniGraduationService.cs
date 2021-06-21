using DevBetterWeb.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IAlumniGraduationService
  {
    Task<List<Member>> CheckIfAnyMemberGraduating(List<Member> members);
    Task<List<string>> GraduateMembers(List<Member> membersToGraduate);
    Task GraduateMembersIfNeeded(AppendOnlyStringList messages);
  }
}
