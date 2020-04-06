using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events
{
    public class PasswordResetEvent : BaseDomainEvent
    {
        public PasswordResetEvent(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public string EmailAddress { get; }
    }
}
