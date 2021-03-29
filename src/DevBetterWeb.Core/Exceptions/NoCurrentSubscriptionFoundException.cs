using System;
using System.Runtime.Serialization;

namespace DevBetterWeb.Core.Exceptions
{
  [Serializable]
  public class NoCurrentSubscriptionFoundException : Exception
  {
    public NoCurrentSubscriptionFoundException()
    {
    }

    public NoCurrentSubscriptionFoundException(string? message) : base(message)
    {
    }

    public NoCurrentSubscriptionFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected NoCurrentSubscriptionFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}
