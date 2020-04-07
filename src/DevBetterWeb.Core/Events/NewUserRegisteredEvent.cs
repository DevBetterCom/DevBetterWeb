using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events
{
    public class NewUserRegisteredEvent : BaseDomainEvent
    {
        public NewUserRegisteredEvent(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public string EmailAddress { get; }
    }
}
