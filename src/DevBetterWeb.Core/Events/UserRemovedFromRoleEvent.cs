using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events
{
    public class UserRemovedFromRoleEvent : BaseDomainEvent
    {
        public UserRemovedFromRoleEvent(string emailAddress, string role)
        {
            EmailAddress = emailAddress;
            Role = role;
        }

        public string EmailAddress { get; }
        public string Role { get; }
    }
}
