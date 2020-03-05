using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events
{
    public class UserAddedToRoleEvent : BaseDomainEvent
    {
        public UserAddedToRoleEvent(string userId, string roleName)
        {
            UserId = userId;
            RoleName = roleName;
        }

        public string UserId { get; }
        public string RoleName { get; }
    }
}
