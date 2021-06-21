using DevBetterWeb.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IAlumniGraduationService
  {
    Task<List<Member>> CheckIfAnyMemberGraduating();
    Task GraduateMembers(List<Member> membersToGraduate);
  }
}
