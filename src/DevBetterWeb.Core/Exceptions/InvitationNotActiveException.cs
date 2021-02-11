using System;
using System.Runtime.Serialization;

namespace DevBetterWeb.Core.Exceptions
{
  [Serializable]
  internal class InvitationNotActiveException : Exception
  {
    public InvitationNotActiveException()
    {
    }

    public InvitationNotActiveException(string? message) : base(message)
    {
    }

    public InvitationNotActiveException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected InvitationNotActiveException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}
