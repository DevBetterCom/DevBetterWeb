using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IMemberLookupService
  {
    Task<Member> GetMemberByEmailAsync(string memberEmail);
  }
}
