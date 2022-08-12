using System;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events;

public class ExceptionEvent : BaseDomainEvent
{
  public ExceptionEvent(Exception exception)
  {
	  Exception = exception;
  }

  public Exception Exception { get; }
}
