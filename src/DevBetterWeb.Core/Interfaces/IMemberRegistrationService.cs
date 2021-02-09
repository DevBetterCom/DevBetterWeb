using DevBetterWeb.Core.Entities;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IMemberRegistrationService
  {
    Task<Member> RegisterMemberAsync(string userId);
  }
}
