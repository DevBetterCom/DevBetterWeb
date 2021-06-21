using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;

namespace DevBetterWeb.Infrastructure.Services
{
  public class AlumniGraduationService : IAlumniGraduationService
  {
    public Task<List<Member>> CheckIfAnyMemberGraduating()
    {
      throw new System.NotImplementedException();
    }

    public Task GraduateMembers(List<Member> membersToGraduate)
    {
      throw new System.NotImplementedException();
    }
  }
}
