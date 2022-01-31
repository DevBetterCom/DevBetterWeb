using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces;

public interface IDailyCheckPingService
{
  Task SendPingIfNeeded(AppendOnlyStringList messages);
  Task PingAdminsAboutAlmostAlumsIfNeeded(AppendOnlyStringList messages);
  Task DeactiveInvitesForExistingMembers(AppendOnlyStringList messages);
}
