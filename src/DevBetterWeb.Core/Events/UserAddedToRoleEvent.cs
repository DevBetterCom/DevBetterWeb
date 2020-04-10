using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events
{
    public class UserAddedToRoleEvent : BaseDomainEvent
    {
        public UserAddedToRoleEvent(string emailAddress, string role)
        {
            EmailAddress = emailAddress;
            Role = role;
        }

        public string EmailAddress { get; }
        public string Role { get; }
    }
}
