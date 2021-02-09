using System;
using System.Runtime.Serialization;

namespace DevBetterWeb.Core.Services
{
  [Serializable]
  internal class InvitationNotFoundException : Exception
  {
    public InvitationNotFoundException()
    {
    }

    public InvitationNotFoundException(string? message) : base(message)
    {
    }

    public InvitationNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected InvitationNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}