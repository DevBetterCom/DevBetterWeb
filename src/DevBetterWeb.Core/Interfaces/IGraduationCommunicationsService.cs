using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Interfaces;

public interface IGraduationCommunicationsService
{
  Task SendGraduationCommunications(Member member);
}
