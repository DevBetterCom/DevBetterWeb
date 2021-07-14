using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IDailyCheckPingService
  {
    Task SendPingIfNeeded(AppendOnlyStringList messages);
    List<Invitation> CheckIfAnyActiveInvitationsRequireUserPing(List<Invitation> invitations);
    List<Invitation> CheckIfAnyActiveInvitationsRequireAdminPing(List<Invitation> invitations);
  }
}
