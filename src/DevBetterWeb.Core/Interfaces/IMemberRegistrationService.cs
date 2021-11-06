using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Interfaces;

public interface IMemberRegistrationService
{
  Task<Member> RegisterMemberAsync(string userId);
}
