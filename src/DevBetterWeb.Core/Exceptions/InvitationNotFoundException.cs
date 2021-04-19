using System;
using System.Runtime.Serialization;

namespace DevBetterWeb.Core.Exceptions
{
  [Serializable]
  public class InvitationNotFoundException : Exception
  {
    public InvitationNotFoundException()
    {
    }

    public InvitationNotFoundException(string? message) : base(message)
    {
    }
  }
}
