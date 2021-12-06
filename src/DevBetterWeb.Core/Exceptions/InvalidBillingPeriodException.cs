using System;
using System.Runtime.Serialization;

namespace DevBetterWeb.Core.Exceptions;

[Serializable]
public class InvalidBillingPeriodException : Exception
{
  public InvalidBillingPeriodException()
  {
  }

  public InvalidBillingPeriodException(string? message) : base(message)
  {
  }

  public InvalidBillingPeriodException(string? message, Exception? innerException) : base(message, innerException)
  {
  }

  protected InvalidBillingPeriodException(SerializationInfo info, StreamingContext context) : base(info, context)
  {
  }
}
