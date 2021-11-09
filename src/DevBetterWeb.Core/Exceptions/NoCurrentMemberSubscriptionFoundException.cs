using System;
using System.Runtime.Serialization;

namespace DevBetterWeb.Core.Exceptions;

[Serializable]
public class NoCurrentMemberSubscriptionFoundException : Exception
{
  public NoCurrentMemberSubscriptionFoundException()
  {
  }

  public NoCurrentMemberSubscriptionFoundException(string? message) : base(message)
  {
  }

  public NoCurrentMemberSubscriptionFoundException(string? message, Exception? innerException) : base(message, innerException)
  {
  }

  protected NoCurrentMemberSubscriptionFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
  {
  }
}
