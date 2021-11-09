using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Interfaces;

public interface IAlumniGraduationService
{
  Task<List<Member>> CheckIfAnyMemberGraduating(List<Member> members);
  Task<List<string>> GraduateMembers(List<Member> membersToGraduate);
  Task GraduateMembersIfNeeded(AppendOnlyStringList messages);
}
