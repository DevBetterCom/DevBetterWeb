using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events
{
  public class UserEmailConfirmedChangedEvent : BaseDomainEvent
  {
    public UserEmailConfirmedChangedEvent(string emailAddress, bool isEmailConfirmed)
    {
      EmailAddress = emailAddress;
      IsEmailConfirmed = isEmailConfirmed;
    }

    public string EmailAddress { get; }
    public bool IsEmailConfirmed { get; }
  }
}
