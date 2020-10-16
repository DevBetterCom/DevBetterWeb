using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events
{
  public class InvalidUserEvent : BaseDomainEvent
  {
    public InvalidUserEvent(string emailAddress)
    {
      EmailAddress = emailAddress;
    }

    public string EmailAddress { get; }
  }
}
