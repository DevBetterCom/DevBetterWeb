using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Interfaces;

public interface IDailyCheckPingService
{
  Task SendPingIfNeeded(AppendOnlyStringList messages);
  Task PingAdminsAboutAlmostAlumsIfNeeded(AppendOnlyStringList messages);
}
