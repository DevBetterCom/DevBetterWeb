using System;

namespace DevBetterWeb.Core.Exceptions
{
  [Serializable]
  public class InvitationNotFoundException : Exception
  {
    public InvitationNotFoundException(string inviteCode) : base($"Could not find invitation with inviteCode {inviteCode}.")
    {
    }
  }
}
