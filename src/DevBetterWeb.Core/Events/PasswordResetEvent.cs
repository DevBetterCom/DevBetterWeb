using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events;

public class PasswordResetEvent : BaseDomainEvent
{
  public PasswordResetEvent(string emailAddress, string callbackUrl)
  {
    EmailAddress = emailAddress;
		CallbackUrl = callbackUrl;
	}

  public string EmailAddress { get; }
	public string CallbackUrl { get; }
}
