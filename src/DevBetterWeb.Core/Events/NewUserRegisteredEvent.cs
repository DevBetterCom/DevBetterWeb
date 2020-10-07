using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events
{
    public class NewUserRegisteredEvent : BaseDomainEvent
    {
        public NewUserRegisteredEvent(string emailAddress, string ipAddress)
        {
            EmailAddress = emailAddress;
            IpAddress = ipAddress;
        }

        public string EmailAddress { get; }
        public string IpAddress { get; }
    }
}
