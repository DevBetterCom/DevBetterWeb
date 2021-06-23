using DevBetterWeb.Core.Entities;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IGraduationCommunicationsService
  {
    Task SendGraduationCommunications(Member member);
  }
}
